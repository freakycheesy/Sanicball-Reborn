using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mirror.Examples.AdditiveLevels;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Mirror
{

    public class AddressableNetworkManager : NetworkManager
    {

        public static SerializedDictionary<string, AssetReference> CompiledAddressableReferences = new();

        [Tooltip("List of scene references")]
        public List<AssetReference> SceneRefs = new();
        public static AddressableNetworkManager AddressableManager { get; private set; }
        public override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            AddressableManager = this;
            CompiledAddressableReferences = new();
            for (int i = 0; i < SceneRefs.Count; i++)
                CompiledAddressableReferences.Add((string)SceneRefs[i].RuntimeKey, SceneRefs[i]);
        }

        public static bool IsValidScene(string sceneName)
        {
            return CompiledAddressableReferences.ContainsKey(sceneName.ToLower());
        }
        public static void AddSceneReference(AssetReference sceneReference)
        {
            CompiledAddressableReferences.Add((string)sceneReference.RuntimeKey, sceneReference);
        }

        public static AssetReference GetSceneFromKey(string sceneRuntimekey)
        {
            if (!CompiledAddressableReferences.TryGetValue(sceneRuntimekey, out AssetReference sceneReference))
                throw new ArgumentException($"Scene with key: {sceneRuntimekey} is not registered in AddressableNetworkManager!", nameof(sceneRuntimekey));
            return sceneReference;
        }
        public static bool TryGetSceneFromKey(string sceneRuntimekey, out AssetReference sceneReference)
        {
            sceneReference = GetSceneFromKey(sceneRuntimekey);
            return sceneReference != null;
        }

        public override void ServerChangeScene(string newSceneKey)
        {
            if (!TryGetSceneFromKey(newSceneKey, out var scene))
                throw new ArgumentException($"Server Loading Scene Error");
            if (string.IsNullOrWhiteSpace(newSceneKey))
            {
                Debug.LogError("ServerChangeScene empty scene name");
                return;
            }

            if (NetworkServer.isLoadingScene && newSceneKey == networkSceneName)
            {
                Debug.LogError($"Scene change is already in progress for {newSceneKey}");
                return;
            }

            // Throw error if called from client
            // Allow changing scene while stopping the server
            if (!NetworkServer.active && newSceneKey != offlineScene)
            {
                Debug.LogError("ServerChangeScene can only be called on an active server.");
                return;
            }

            CurrentCoroutine ??= StartCoroutine(LoadServerAddressableScene(scene, newSceneKey));
        }

        private Coroutine CurrentCoroutine = null;

        IEnumerator LoadServerAddressableScene(AssetReference scene, string newSceneRuntimeKey)
        {
            NetworkClient.isLoadingScene = true;

            NetworkServer.SetAllClientsNotReady();
            networkSceneName = scene.SubObjectName;
            // Let server prepare for scene change
            OnServerChangeScene(newSceneRuntimeKey);
            // set server flag to stop processing messages while changing scenes
            // it will be re-enabled in FinishLoadScene.
            NetworkServer.isLoadingScene = true;
            var handle = Addressables.LoadSceneAsync(scene);
            yield return handle;
            // ServerChangeScene can be called when stopping the server
            // when this happens the server is not active so does not need to tell clients about the change
            if (NetworkServer.active)
            {
                // notify all clients about the new scene
                NetworkServer.SendToAll(new SceneMessage
                {
                    sceneName = newSceneRuntimeKey,
                    customHandling = true,
                });
            }

            startPositionIndex = 0;
            startPositions.Clear();
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            // if client is remote, send 2nd scene message to load the Addressable Online scene
            if (conn is LocalConnectionToClient == false)
                conn.Send(new SceneMessage { sceneName = networkSceneName, customHandling = true });
        }

        public override void OnClientChangeScene(string newSceneKey, SceneOperation sceneOperation, bool customHandling)
        {
            if (customHandling)
            {
                // Block processing of network messages
                if (!TryGetSceneFromKey(newSceneKey, out var scene))
                    throw new ArgumentException($"Client Loading Scene Error");
                NetworkClient.isLoadingScene = true;
                CurrentCoroutine ??= StartCoroutine(LoadClientAddressableScene(scene));
            }
        }

        IEnumerator LoadClientAddressableScene(AssetReference scene)
        {     
            NetworkClient.isLoadingScene = true;
            

            var handle = Addressables.LoadSceneAsync(scene);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                networkSceneName = scene.SubObjectName;
                handle.Result.ActivateAsync();
            }
            else
            {
                // Enhanced error logging
                Debug.LogError($"Failed to load Addressable scene: {scene.SubObjectName}");
                Debug.LogError($"Status: {handle.Status}");

                if (handle.OperationException != null)
                {
                    Debug.LogError($"Exception: {handle.OperationException}");
                    Debug.LogError($"Stack Trace:\n{handle.OperationException.StackTrace}");
                }
            }

            NetworkClient.isLoadingScene = false;
            OnClientSceneChanged();
        }
    }

}
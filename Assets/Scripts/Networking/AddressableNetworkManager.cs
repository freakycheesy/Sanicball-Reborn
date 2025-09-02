using System;
using System.Collections.Generic;
using System.IO;
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

        public SerializedDictionary<string, AssetReference> CompiledAddressableReferences;

        [Tooltip("List of scene references")]
        public List<AssetReference> SceneRefs = new();
        public static AddressableNetworkManager AddressableManager { get; private set; }
        public override void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            AddressableManager = this;
            CompiledAddressableReferences = new();
            for (int i = 0; i < SceneRefs.Count; i++)
                CompiledAddressableReferences.Add((string)SceneRefs[i].RuntimeKey, SceneRefs[i]);
        }

        public AssetReference GetSceneRefByName(string sceneName)
        {
            sceneName = Path.GetFileNameWithoutExtension(sceneName).ToLower();
            CompiledAddressableReferences.TryGetValue(sceneName, out AssetReference assetReference);
            return assetReference;
        }

        public AssetReference GetSceneRefByGuid(string guid)
        {
            foreach (var item in CompiledAddressableReferences)
            {
                if (item.Value.RuntimeKeyIsValid() && item.Value.RuntimeKey.ToString() == guid)
                    return item.Value;
            }

            return null;
        }

        /// <summary>
        /// Resets performed loading operations for a given load queue.
        /// </summary>
        private void ResetProcessor()
        {
            loadingSceneAsync.Release();
        }

        /// <summary>
        /// Checks if we have a loadable scene in our compiled scene refs.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static bool IsValidScene(string sceneName)
        {
            var proc = NetworkManager.singleton as AddressableNetworkManager;
            if (proc == null) return false;

            return proc.CompiledAddressableReferences.ContainsKey(sceneName.ToLower());
        }

        public override void ServerChangeScene(string sceneName)
        {
            ServerChangeScene(sceneName, LoadSceneMode.Single);
        }
        public new static AsyncOperationHandle<SceneInstance> loadingSceneAsync;
        private string ServerChangeScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            sceneName = Path.GetFileNameWithoutExtension(sceneName);

            // Try get reference
            if (!CompiledAddressableReferences.TryGetValue(sceneName, out AssetReference sceneReference))
                throw new ArgumentException($"Scene with name: {sceneName} is not registered in AddressableSceneProcessor!", nameof(sceneName));
            NetworkServer.SetAllClientsNotReady();
            networkSceneName = sceneName;

            NetworkServer.isLoadingScene = true;
            loadingSceneAsync = Addressables.LoadSceneAsync(sceneReference, mode, false);

            if (NetworkServer.active)
            {
                // notify all clients about the new scene
                NetworkServer.SendToAll(new SceneMessage
                {
                    sceneName = sceneName
                });
            }

            startPositionIndex = 0;
            startPositions.Clear();
            return sceneName;
        }

        protected override void ClientChangeScene(string newSceneName, SceneOperation sceneOperation = SceneOperation.Normal, bool customHandling = false)
        {
            //base.ClientChangeScene(newSceneName, sceneOperation, customHandling);
            if (string.IsNullOrWhiteSpace(newSceneName))
            {
                Debug.LogError("ClientChangeScene empty scene name");
                return;
            }

            //Debug.Log($"ClientChangeScene newSceneName: {newSceneName} networkSceneName{networkSceneName}");

            // Let client prepare for scene change
            OnClientChangeScene(newSceneName, sceneOperation, customHandling);

            // After calling OnClientChangeScene, exit if server since server is already doing
            // the actual scene change, and we don't need to do it for the host client
            if (NetworkServer.active)
                return;

            // set client flag to stop processing messages while loading scenes.
            // otherwise we would process messages and then lose all the state
            // as soon as the load is finishing, causing all kinds of bugs
            // because of missing state.
            // (client may be null after StopClient etc.)
            // Debug.Log("ClientChangeScene: pausing handlers while scene is loading to avoid data loss after scene was loaded.");
            NetworkClient.isLoadingScene = true;

            // Cache sceneOperation so we know what was requested by the
            // Scene message in OnClientChangeScene and OnClientSceneChanged
            //clientSceneOperation = sceneOperation;

            // scene handling will happen in overrides of OnClientChangeScene and/or OnClientSceneChanged
            // Do not call FinishLoadScene here. Custom handler will assign loadingSceneAsync and we need
            // to wait for that to finish. UpdateScene already checks for that to be not null and isDone.
            if (customHandling)
                return;
            if (!CompiledAddressableReferences.TryGetValue(newSceneName, out AssetReference sceneReference))
                throw new ArgumentException($"Scene with name: {newSceneName} is not registered in AddressableSceneProcessor!", nameof(newSceneName));
            switch (sceneOperation)
            {
                case SceneOperation.Normal:
                    loadingSceneAsync = Addressables.LoadSceneAsync(sceneReference);
                    break;
                case SceneOperation.LoadAdditive:
                    // Ensure additive scene is not already loaded on client by name or path
                    // since we don't know which was passed in the Scene message
                    if (!IsValidScene(newSceneName))
                        loadingSceneAsync = Addressables.LoadSceneAsync(sceneReference, LoadSceneMode.Additive);
                    else
                    {
                        Debug.LogWarning($"Scene {newSceneName} is already loaded");

                        // Reset the flag that we disabled before entering this switch
                        NetworkClient.isLoadingScene = false;
                    }
                    break;
                case SceneOperation.UnloadAdditive:
                    // Ensure additive scene is actually loaded on client by name or path
                    // since we don't know which was passed in the Scene message
                    if (!IsValidScene(newSceneName))
                        loadingSceneAsync = Addressables.UnloadSceneAsync(loadingSceneAsync.Result, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                    else
                    {
                        Debug.LogWarning($"Cannot unload {newSceneName} with UnloadAdditive operation");

                        // Reset the flag that we disabled before entering this switch
                        NetworkClient.isLoadingScene = false;
                    }
                    break;
            }

            // don't change the client's current networkSceneName when loading additive scene content
            if (sceneOperation == SceneOperation.Normal)
                networkSceneName = newSceneName;
        }

    }

}
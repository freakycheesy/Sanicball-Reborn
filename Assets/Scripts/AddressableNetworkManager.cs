using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Scenes
{

    public sealed class AddressableNetworkManager : NetworkManager
    {

        /// <summary>
        /// Cached (scene handle, async handles) loaded by the processor, so we can use them for unloading later.
        /// </summary>
        private readonly Dictionary<int, AsyncOperationHandle<SceneInstance>> _loadedScenesByHandle = new(4);
        /// <summary>
        /// Cached loaded scenes known by this processor.
        /// </summary>
        private readonly List<Scene> _loadedScenes = new(4);
        /// <summary>
        /// Current load/unload operations in queue.
        /// </summary>
        private readonly List<AsyncOperationHandle<SceneInstance>> _loadingAsyncOperations = new(4);
        /// <summary>
        /// The current addressables load operation being handled.
        /// </summary>
        private AsyncOperationHandle<SceneInstance> _currentAsyncOperation;
        /// <summary>
        /// Basic async operation for unloading scenes that the processor doesn't know about, i.e. offline scenes.
        /// </summary>
        private AsyncOperation _currentBasicAsyncOperation;
        /// <summary>
        /// Dictionary wrapper of raw scene references so we can get them by scene name.
        /// </summary>
        public SerializedDictionary<string, AssetReference> CompiledAddressableReferences;

        [Tooltip("List of scene references")]
        public List<AssetReference> SceneRefs = new();

        public override void Awake()
        {
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
            _currentAsyncOperation = default;
            loadingSceneAsync = null;
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

        public override void Sere(string sceneName, LoadSceneParameters parameters)
        {
            sceneName = Path.GetFileNameWithoutExtension(sceneName);

            // Try get reference
            if (!CompiledAddressableReferences.TryGetValue(sceneName, out AssetReference sceneReference))
                throw new ArgumentException($"Scene with name: {sceneName} is not registered in AddressableSceneProcessor!", nameof(sceneName));

            // load scene with Addressables
            //  =>  only the static method takes in LoadSceneParameters
            AsyncOperationHandle<SceneInstance> loadHandle = Addressables.LoadSceneAsync(sceneReference, parameters, false);

            // And register this handle in systems
            _loadingAsyncOperations.Add(loadHandle);
            _currentAsyncOperation = loadHandle;
        }

        public override void BeginUnloadAsync(Scene scene)
        {
            if (_loadedScenesByHandle.TryGetValue(scene.handle, out var loadHandle))
            {
                AsyncOperationHandle<SceneInstance> unloadHandle = Addressables.UnloadSceneAsync(loadHandle, false);
                _currentAsyncOperation = unloadHandle;
                _loadedScenes.Remove(scene);
                _loadedScenesByHandle.Remove(scene.handle);
            }
            else
            {
                // maybe it was loaded independently
                _currentBasicAsyncOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
            }
        }

        public override bool IsPercentComplete()
        {
            bool completed;
            if (_currentBasicAsyncOperation != null)
            {
                completed = GetPercentComplete() >= 0.9;
                if (completed)
                    _currentBasicAsyncOperation = null;
                return completed;
            }

            if (_currentAsyncOperation.IsValid())
            {
                completed = _currentAsyncOperation.IsDone;

                // cache the scene here since FN's scene manager doesnt
                // support addressable handles in their loading loop
                if (completed)
                {
                    AddLoadedScene(_currentAsyncOperation);
                }

                return completed;
            }

            Debug.LogError("Addressable Scene Processor: No Async operations running", this);
            return false;
        }

        public override float GetPercentComplete()
        {
            float percent = 0f;

            if (_currentBasicAsyncOperation != null)
            {
                percent = _currentBasicAsyncOperation.progress;
            }
            else if (_currentAsyncOperation.IsValid())
            {
                percent = _currentAsyncOperation.PercentComplete;
            }

            return percent;
        }

        // this doesnt even do anything
        public override List<Scene> GetLoadedScenes() => _loadedScenes;

        /// <summary>
        /// Caches a loaded scene.
        /// </summary>
        /// <param name="scene">Loaded scene</param>
        /// <param name="loadHandle">Handle that loaded the scene</param>
        public void AddLoadedScene(AsyncOperationHandle<SceneInstance> loadHandle)
        {
            Scene scene = _currentAsyncOperation.Result.Scene;
            if (_loadedScenesByHandle.ContainsKey(scene.handle))
            {
                Debug.LogWarning("Already added scene with handle: " + scene.handle);
                return;
            }

            _loadedScenes.Add(scene);
            _loadedScenesByHandle.Add(scene.handle, loadHandle);
        }

        public override void ActivateLoadedScenes()
        {
            foreach (var loadingAsyncOp in _loadingAsyncOperations)
            {
                loadingAsyncOp.Result.ActivateAsync();
            }
        }

        public override IEnumerator AsyncsIsDone()
        {
            bool notDone = true;

            while (notDone)
            {
                notDone = false;

                foreach (AsyncOperationHandle<SceneInstance> ao in _loadingAsyncOperations)
                {
                    if (!ao.IsDone)
                    {
                        notDone = true;
                        break;
                    }
                }

                yield return null;
            }
        }
    }

}
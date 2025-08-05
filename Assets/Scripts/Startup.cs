using System.Collections.Generic;
using System.Linq;
using Sanicball.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Sanicball
{
    public class Startup : MonoBehaviour
    {
        public UI.Intro intro;
        public CanvasGroup setNicknameGroup;
        public InputField nicknameField;
        public static bool isReady = false;
        public void ValidateNickname()
        {
            if (nicknameField.text.Trim() != "" && isReady)
            {
                setNicknameGroup.alpha = 0f;
                ActiveData.GameSettings.nickname = nicknameField.text;
                intro.enabled = true;
            }
        }

        private void Awake()
        {
            isReady = false;
            LoadGame();         
        }

        private void LoadGame()
        {
            var defaultAssets = Addressables.LoadAssetsAsync<Object>("default", LoadCallback);
            defaultAssets.Completed += OnComplete;
        }

        private void LoadCallback(Object @object)
        {
            Debug.Log($"Loaded:{@object.name}");
        }

        private void OnComplete(AsyncOperationHandle<IList<Object>> handle)
        {
            isReady = true;
            Debug.Log("Completed Loading Addressables");
            if (string.IsNullOrEmpty(ActiveData.GameSettings.nickname) || ActiveData.GameSettings.nickname == "Player")
            {
                //Set nickname before continuing
                setNicknameGroup.alpha = 1f;
            }
            else
            {
                setNicknameGroup.alpha = 0f;
                intro.enabled = true;
            }
        }
    }
}
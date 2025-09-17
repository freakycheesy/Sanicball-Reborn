using System.Collections.Generic;
using System.Linq;
using Sanicball.Data;
using UnityEngine;
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
            isReady = true;
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
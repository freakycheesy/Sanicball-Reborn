using Sanicball.Logic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sanicball.UI
{
    public class PauseMenu : MonoBehaviour
    {
        private const string pauseTag = "Pause";
        [SerializeField]
        private GameObject firstSelected = null;

        [SerializeField]
        private Button contextSensitiveButton;
        [SerializeField]
        private Text contextSensitiveButtonLabel;

        private bool mouseWasLocked;

        public static bool GamePaused { get { return GameObject.FindWithTag(pauseTag); } }

        public bool OnlineMode { get; set; }
        public static PauseMenu Instance;
        private void Awake()
        {
            Instance = this;
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                mouseWasLocked = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void Start()
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(firstSelected);
            if (!OnlineMode)
            {
                Time.timeScale = 0;
                AudioListener.pause = true;
            }

            if (SceneManager.GetActiveScene().name == "Lobby")
            {
                contextSensitiveButtonLabel.text = "Change match settings";
                contextSensitiveButton.onClick.AddListener(MatchSettings);
                if (OnlineMode)
                {
                    contextSensitiveButton.interactable = false;
                }
            }
            else
            {
                contextSensitiveButtonLabel.text = "Return to lobby";
                contextSensitiveButton.onClick.AddListener(BackToLobby);
            }
        }

        public void Close()
        {
            if (mouseWasLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (!OnlineMode)
            {
                Time.timeScale = 1;
                AudioListener.pause = false;
            }
        }

        public void MatchSettings()
        {
            LobbyReferences.Active.MatchSettingsPanel.Show();
            Close();
        }

        public void BackToLobby()
        {
            if (MatchManager.Instance)
            {
                MatchManager.Instance.RequestLoadLobby();
                Close();
            }
            else
            {
                Debug.LogError("Cannot return to lobby: no match manager found to handle the request. Something is broken!");
            }
        }

        public void QuitMatch()
        {
            if (MatchManager.Instance)
            {
                MatchManager.Instance.QuitMatch();
            }
            else
            {
                Addressables.LoadSceneAsync(MatchManager.Instance.menuScene);
            }
        }
    }
}
using SanicballCore;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
    public class MainMenuPanel : MonoBehaviour
    {
        [SerializeField]
        private Text versionNameField = null;
        [SerializeField]
        private Text taglineField = null;

        private SlideCanvasGroup activePanel;

        public void SetActivePanel(SlideCanvasGroup panel)
        {
            if (activePanel == null)
            {
                //Opening new panel
                panel.Open();
                activePanel = panel;
            }
            else
            {
                //Changing panel
                if (activePanel != panel)
                {
                    CloseActivePanel();
                    panel.Open();
                    activePanel = panel;
                }
                else
                {
                    //Closing active panel
                    CloseActivePanel();
                }
            }
        }

        public void CloseActivePanel()
        {
            activePanel.Close();
            activePanel = null;
        }
        public class GameVersion
        {
            public const float AS_FLOAT = 0.82f;

            public const bool IS_TESTING = false;

            public const string AS_STRING = "PEAK";

            public const string TAGLINE = "THE BEST MEME GAME REVAMPED! BETTER THEN ANY BRAINROT SLOP!";
        }

        private void Start()
        {
            versionNameField.text = GameVersion.AS_STRING;
            taglineField.text = GameVersion.TAGLINE;
        }

        private void Update()
        {
            if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1)) && Resources.FindObjectsOfTypeAll<Popup>().Length <= 0)
            {
                if (activePanel != null)
                {
                    CloseActivePanel();
                }
                else
                {
                    Application.Quit();
                }
            }
        }
    }
}
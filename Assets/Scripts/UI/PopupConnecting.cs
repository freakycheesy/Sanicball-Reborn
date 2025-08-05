using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
    public class PopupConnecting : MonoBehaviour
    {
        [SerializeField]
        private Text titleField = null;
        [SerializeField]
        private Image spinner = null;
        public static PopupConnecting Instance;
        void Start()
        {
            Instance = this;
        }

        public void ShowMessage(string text)
        {
            titleField.text = text;
            spinner.enabled = false;
        }
    }
}
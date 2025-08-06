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
        void OnEnable()
        {
            if (Instance) { Destroy(this); return; }
            Instance = this;
        }
        public static void ShowMessage(string text)
        {
            Instance?.InstanceShowMessage(text);
        }
        private void InstanceShowMessage(string text)
        {
            titleField.text = text;
            spinner.enabled = false;
        }
    }
}
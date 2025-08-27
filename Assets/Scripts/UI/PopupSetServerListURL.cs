using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
    [RequireComponent(typeof(Popup))]
    public class PopupSetServerListURL : MonoBehaviour
    {
        public InputField url;

        private OptionsPanel optionsPanel;

        public void Validate()
        {
			string u = url.text.Trim();
            GetComponent<Popup>().Close();
        }

        private void Start()
        {
            optionsPanel = OptionsPanel.Instance;

            if (!optionsPanel)
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}
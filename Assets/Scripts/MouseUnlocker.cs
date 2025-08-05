using UnityEngine;

namespace Sanicball
{
    public class MouseUnlocker : MonoBehaviour
    {
        public static MouseUnlocker Instance;
        private void Start()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}

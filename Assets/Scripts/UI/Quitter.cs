using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Sanicball.UI
{
    public class Quitter : MonoBehaviour
    {
        public void Quit()
        {
            //Beb
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }
    }
}

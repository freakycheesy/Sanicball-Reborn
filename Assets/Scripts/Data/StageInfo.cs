using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Sanicball.Data
{
    [System.Serializable]
    public class StageInfo
    {
        public string BARCODE;
        public string name;
        public SceneReference scene;
        public Sprite picture;
        public GameObject overviewPrefab;
    }
}

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Sanicball.Data
{
    [System.Serializable]
    public class StageInfo
    {
        public string BARCODE;
        public string name;
        public AssetReference scene;
        public Sprite picture;
        public GameObject overviewPrefab;
    }
}

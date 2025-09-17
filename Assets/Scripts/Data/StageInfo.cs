using Mirror;
using UnityEngine;

namespace Sanicball.Data
{
    [System.Serializable]
    public class StageInfo
    {
        [HideInInspector] public int id = 0;
        public string BARCODE;
        public string name;
        [Scene] public string scene;
        public Sprite picture;
        public GameObject overviewPrefab;
    }
}

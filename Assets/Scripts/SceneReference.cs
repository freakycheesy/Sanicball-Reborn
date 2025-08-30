using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Sanicball.Data
{
    [System.Serializable]
    public class SceneReference : AssetReference
    {
        public SceneReference(string guid) : base(guid) { }
        public override bool ValidateAsset(string path)
        {
            return path.EndsWith(".unity");
        }
    }
}
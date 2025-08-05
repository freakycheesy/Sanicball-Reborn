using UnityEngine;

namespace Sanicball
{
    public class RandomTexture : MonoBehaviour
    {
        public Texture[] textures;
        private int current;
        private new Renderer renderer;
        public int GetCurrentTexture()
        {
            return current;
        }

        public void SetTexture(int i)
        {
            var texture = textures[i];
            renderer.material.mainTexture = texture;
            current = i;
        }

        private void Start()
        {
            renderer = GetComponent<Renderer>();
            SwitchTexture();
        }

        private void SwitchTexture()
        {
            int m = Random.Range(0, textures.Length);
            renderer.material.mainTexture = textures[m];
            current = m;
        }
    }
}
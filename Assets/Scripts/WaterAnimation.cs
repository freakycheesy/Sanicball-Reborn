using UnityEngine;

namespace Sanicball
{
    [ExecuteInEditMode]
    public class WaterAnimation : MonoBehaviour
    {
        public const float startSpeed = 0.05f;
        public Vector2 speed = new(startSpeed, startSpeed);
        private Vector2 offset = new();
        void Start()
        {
            offset = new();
        }
        private void Update()
        {
            offset += new Vector2(speed.x * Time.deltaTime, speed.y * Time.deltaTime);
            if (offset.x >= 1)
            {
                offset += new Vector2(-1, 0);
            }
            if (offset.y >= 1)
            {
                offset += new Vector2(0, -1);
            }
            GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
            GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_BaseMap", offset);
        }
    }
}
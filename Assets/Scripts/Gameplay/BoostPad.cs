using Sanicball;
using UnityEngine;

namespace Sanicball.Gameplay
{

    public class BoostPad : AutoPlacement
    {
        [SerializeField]
        private float speed = 1f;

        [SerializeField]
        private float speedLimit = 200f;

        private float offset;
        
        private void Update()
        {
            //Animate the panel on the boost pad
            offset -= 5f * Time.deltaTime;
            if (offset <= 0f)
            {
                offset += 1f;
            }
            GetComponent<Renderer>().materials[1].SetTextureOffset("_MainTex", new Vector2(0f, offset));
        }

        private void OnTriggerEnter(Collider other)
        {
            var bc = other.GetComponent<Ball>();
            if (bc != null)
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb)
                {
                    float speed = rb.linearVelocity.magnitude;
                    speed = Mathf.Min(speed + this.speed, speedLimit);
                    rb.linearVelocity = transform.rotation * Vector3.forward * speed;

                    AudioSource aSource = GetComponent<AudioSource>();
                    if (aSource)
                    {
                        aSource.Play();
                    }
                }
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
namespace Sanicball.Gameplay
{
    public class LocalController : MonoBehaviour
    {
        public static List<LocalController> Controllers = new();
        void OnEnable()
        {
            Controllers.Add(this);
        }
        void OnDisable()
        {
            Controllers.Remove(this);
        }

        public virtual void OnUpdate()
        {

        }
        public virtual void OnFixedUpdate()
        {

        }
    }
}
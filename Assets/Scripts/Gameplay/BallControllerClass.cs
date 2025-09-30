using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Sanicball.Gameplay
{
    public class GlobalBallController : MonoBehaviour
    {
        public static GlobalBallController Instance { get; private set; }
        [SerializeField] private List<IBallBehaviour> ballBehaviours = new();
        public static List<IBallBehaviour> BallBehaviours { get => Instance.ballBehaviours; set => Instance.ballBehaviours = value; }    
        void Start()
        {
            Instance = this;
        }
        void FixedUpdate()
        {
            ballBehaviours.ToList().ForEach(a => a.OnFixedUpdate());
        }

        void Update()
        {
            ballBehaviours.ToList().ForEach(a => a.OnUpdate());
        }
        public class LocalBallBehaviour : MonoBehaviour, IBallBehaviour
        {
            void OnEnable() => Instance.ballBehaviours.Add(this);
            void OnDisable() => Instance.ballBehaviours.Remove(this);
            public virtual void OnUpdate()
            {
                
            }
            public virtual void OnFixedUpdate()
            {
                
            }
        }

        public interface IBallBehaviour
        {
            public abstract void OnUpdate();
            public abstract void OnFixedUpdate();
        }
    }
}
using UnityEngine;
using static Sanicball.Gameplay.GlobalBallController;

namespace Sanicball.Gameplay
{
    [RequireComponent(typeof(Ball))]
    public abstract class BallControlBase : MonoBehaviour, IBallBehaviour
    {
        [SerializeField] protected Ball ball;

        public virtual void OnFixedUpdate()
        {
            
        }

        public virtual void OnUpdate()
        {
  
        }

        private void Start()
        {
            TryGetComponent(out ball);
        }
    }
}
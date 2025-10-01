using UnityEngine;

namespace Sanicball.Gameplay
{
    [RequireComponent(typeof(Ball))]
    public abstract class BallControlBase : LocalController
    {
        [SerializeField] protected Ball ball;

        private void Start()
        {
            TryGetComponent(out ball);
        }
    }
}
using Sanicball.Gameplay;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Logic
{
    public abstract class BallSpawner : MonoBehaviour
    {
        public abstract string SpawnerKey { get; }
        [SerializeField]
        private Ball ballPrefab = null;

        protected Ball SpawnBall(Vector3 position, Quaternion rotation, BallType ballType, ControlType ctrlType, int character, string nickname, Object context = null)
        {
            var ball = (Ball)Instantiate(ballPrefab, position, rotation);
            ball.Init(ballType, ctrlType, character, nickname);
            Debug.Log($"[{SpawnerKey}] Spawned Ball: {context.name} ({context.GetType().FullName})", context);
            return ball;
        }
    }
}
using Mirror;
using Sanicball.Gameplay;
using SanicballCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sanicball.Logic
{
    public abstract class BallSpawner : MonoBehaviour
    {
        [SerializeField]
        private Ball ballPrefab = null;

        protected Ball SpawnBall(Vector3 position, Quaternion rotation, BallType ballType, ControlType ctrlType, int character, string nickname, NetworkConnection connection)
        {
            var ball = Instantiate(ballPrefab, position, rotation);
            ball.Init(ballType, ctrlType, character, nickname);
            NetworkServer.Spawn(ball.netIdentity.gameObject, connection as NetworkConnectionToClient);
            return ball;
        }
    }
}
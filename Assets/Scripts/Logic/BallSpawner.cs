using FishNet;
using FishNet.Connection;
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
            InstanceFinder.ServerManager.Spawn(ball.NetworkObject, connection, SceneManager.GetSceneAt(1));
            return ball;
        }
    }
}
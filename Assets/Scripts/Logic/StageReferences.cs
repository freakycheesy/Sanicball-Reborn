using System.Collections.Generic;
using System.Linq;
using Sanicball.Gameplay;
using UnityEngine;

namespace Sanicball.Logic
{
    public class StageReferences : MonoBehaviour
    {
        public List<Checkpoint> checkpoints = new();

        public RaceBallSpawner spawnPoint;

        public List<CameraOrientation> waitingCameraOrientations = new();

        public EndOfMatch endOfMatchHandler;

        public static StageReferences Active
        {
            get; private set;
        }

        private void Start()
        {
            Active = this;
            if(checkpoints.Count <= 0) checkpoints = Resources.FindObjectsOfTypeAll<Checkpoint>().ToList();
            spawnPoint = RaceBallSpawner.Instance;
            if (waitingCameraOrientations.Count <= 0) waitingCameraOrientations = Resources.FindObjectsOfTypeAll<CameraOrientation>().ToList();
            endOfMatchHandler = EndOfMatch.Instance;
        }
    }
}
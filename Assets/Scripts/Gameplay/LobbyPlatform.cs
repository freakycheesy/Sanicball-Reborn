using UnityEngine;

namespace Sanicball.Gameplay
{
    public class LobbyPlatform : MonoBehaviour
    {
        public ConfigurableJoint joint;
        public Vector3 lowerPos = new(0, 1f, 0);
        public float baseDelay = 0.5f;
        private JointDrive yDrive;

        public void Activate()
        {
            joint.targetPosition = lowerPos;
            Invoke(nameof(GoToBase), baseDelay);
        }

        private void GoToBase()
        {
            float offset = 0.001f;
            joint.targetPosition = new Vector3(offset, offset, offset);
            joint.yDrive = yDrive;
        }
        private void Start()
        {
            if (!TryGetComponent(out joint)) { Debug.LogError("No Config Joint"); return; }
            joint.targetPosition = lowerPos;
            yDrive = joint.yDrive;
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.position.y > transform.position.y) return;
            joint.targetPosition = lowerPos * 5;
            Invoke(nameof(GoToBase), baseDelay);
        }
    }
}

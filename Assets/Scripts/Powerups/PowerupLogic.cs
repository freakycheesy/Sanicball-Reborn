using UltEvents;
using UnityEngine;

namespace Sanicball.Powerups
{
    [CreateAssetMenu(fileName = "PowerupLogic", menuName = "Sanicball/Powerup Logic")]
    public class PowerupLogic : ScriptableObject, IPlayerModifiers
    {
        [HideInInspector] public LocalPowerupManager manager;
        public Sprite icon;
        public UltEvent onPickupPowerupEvent;
        public UltEvent onPowerupUpdateEvent;
        public UltEvent onLostPowerupEvent;

        public float powerUpDuration = 10;
        public bool LoseAllPowerupWhenLost = true;

        public void AddForce(Vector3 force, ForceMode mode) => manager.AddForce(force, mode);

        public void ResetDamping(bool resetLinear = true, bool resetAngular = true) => manager.ResetDamping(resetLinear, resetAngular);

        public void ResetJumpHeight() => manager.ResetJumpHeight();

        public void ResetMass() => manager.ResetMass();

        public void ResetSpeed() => manager.ResetSpeed();
        public void SetDamping(float linearDamping = -1, float angularDamping = -1) => manager.SetDamping(linearDamping, angularDamping);

        public void SetJumpHeight(float jumpHeight) => manager.SetJumpHeight(jumpHeight);
        public void SetMass(float mass) => manager.SetMass(mass);

        public void SetSpeed(float speed) => manager.SetSpeed(speed);

    }

    public interface IPlayerModifiers
    {
        public void SetSpeed(float speed);
        public void ResetSpeed();
        public void SetJumpHeight(float jumpHeight);
        public void ResetJumpHeight();
        public void SetMass(float mass);
        public void ResetMass();
        public void SetDamping(float linearDamping = -1, float angularDamping = -1);
        public void ResetDamping(bool resetLinear = true, bool resetAngular = true);
        public void AddForce(Vector3 force, ForceMode mode);
    }
}
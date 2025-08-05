using System;
using Sanicball.Gameplay;
using UltEvents;
using UnityEngine;

namespace Sanicball.Powerups
{
    public class LocalPowerupManager : MonoBehaviour, IPlayerModifiers
    {
        private DefaultValues _defaultValues = new();
        public PowerupLogic selectedPowerup { get; private set; }
        private UltEvent _onPickupPowerupEvent;
        private UltEvent _onPowerupUpdateEvent;
        private UltEvent _onLostPowerupEvent;
        private Ball _ball;
        private Rigidbody _rb;

        void Start()
        {
            Invoke(nameof(Setup), 1);        
        }

        private void Setup()
        {
            TryGetComponent(out _ball);
            TryGetComponent(out _rb);
            _defaultValues = new()
            {
                speed = _ball.characterStats.rollSpeed,
                jumpHeight = _ball.characterStats.jumpHeight,
                mass = _rb.mass,
                linearDamp = _rb.linearDamping,
                angularDamp = _rb.angularDamping,
            };
        }


        public void PickedUp(PowerupLogic powerup)
        {
            if (selectedPowerup) return;
            selectedPowerup = powerup;
            _onPickupPowerupEvent = selectedPowerup.onPickupPowerupEvent;
            _onPowerupUpdateEvent = selectedPowerup.onPowerupUpdateEvent;
            _onLostPowerupEvent = selectedPowerup.onLostPowerupEvent;

            _onPickupPowerupEvent.Invoke();
            if(powerup.powerUpDuration <= 255) Invoke(nameof(LosePowerup), powerup.powerUpDuration);
        }

        void Update()
        {
            if (selectedPowerup)
            {
                _onPowerupUpdateEvent?.Invoke();
            }
        }

        public void LosePowerup()
        {
            if (selectedPowerup.LoseAllPowerupWhenLost) ResetAllValues();
            _onLostPowerupEvent.Invoke();
            selectedPowerup = null;
        }

        private void ResetAllValues()
        {
            ResetDamping();
            ResetJumpHeight();
            ResetMass();
            ResetSpeed();
        }


        public void ResetDamping(bool resetLinear = true, bool resetAngular = true)
        {
            if (resetLinear) _rb.linearDamping = _defaultValues.linearDamp;
            if (resetAngular) _rb.angularDamping = _defaultValues.angularDamp;
        }

        public void ResetJumpHeight()
        {
            _ball.characterStats.jumpHeight = _defaultValues.jumpHeight;
        }

        public void ResetMass()
        {
            _rb.mass = _defaultValues.mass;
        }

        public void ResetSpeed()
        {
            _ball.characterStats.rollSpeed = _defaultValues.speed;
        }

        public void SetDamping(float linearDamping = -1, float angularDamping = -1)
        {
            if (linearDamping >= 0) _rb.linearDamping = linearDamping;
            if (angularDamping >= 0) _rb.angularDamping = angularDamping;
        }

        public void SetJumpHeight(float jumpHeight)
        {
            _ball.characterStats.jumpHeight = jumpHeight;
        }

        public void SetMass(float mass)
        {
            _rb.mass = mass;
        }

        public void SetSpeed(float speed)
        {
            _ball.characterStats.rollSpeed = speed;
        }

        public void AddForce(Vector3 force, ForceMode mode)
        {
            _rb.AddForce(force + _ball.Up, mode);
        }

    }

    [Serializable]
    public struct DefaultValues
    {
        public float mass, speed, jumpHeight, linearDamp, angularDamp;
    }
}
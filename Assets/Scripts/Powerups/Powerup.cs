using Sanicball.Data;
using Sanicball.Gameplay;
using UltEvents;
using UnityEngine;

namespace Sanicball.Powerups
{
    [RequireComponent(typeof(Collider))]
    public class Powerup : MonoBehaviour
    {
        public PowerupLogic containedPowerup;
        public bool Disabled { get; private set; }
        public UltEvent OnPickup;
        public UltEvent OnRespawn;
        public float respawnTime = 2f;
        public SpriteRenderer icon;

        void Start()
        {
            RandomisePowerup();
        }

        private void RandomisePowerup()
        {
            containedPowerup = ActiveData.Powerups[Random.Range(0, ActiveData.Powerups.Count - 1)];
            icon.sprite = containedPowerup.icon;
        }


        void OnTriggerEnter(Collider other)
        {
            if (Disabled) return;
            if (other.attachedRigidbody.TryGetComponent(out Ball ball)) if (ball.TryGetComponent(out LocalPowerupManager manager))
                {
                    if (manager.selectedPowerup) return;
                    Disabled = true;
                    containedPowerup.manager = manager;
                    manager.PickedUp(containedPowerup);
                    OnPickup.Invoke();
                    Invoke(nameof(OnReset), respawnTime);
                }
        }

        private void OnReset()
        {
            Disabled = false;
            OnRespawn.Invoke();
        }

    }
}
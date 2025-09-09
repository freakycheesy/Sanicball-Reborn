using Mirror;
using Sanicball.Data;
using Sanicball.Gameplay;
using UltEvents;
using UnityEngine;

namespace Sanicball.Powerups
{
    [RequireComponent(typeof(Collider))]
    public class Powerup : NetworkBehaviour
    {
        public PowerupLogic containedPowerup;
        public bool Disabled { get; private set; }
        public UltEvent OnPickup;
        public UltEvent OnRespawn;
        public float respawnTime = 2f;
        public SpriteRenderer icon;
        public GameObject Renderer;
        public ParticleSystem Particles;

        public override void OnStartServer()
        {
            base.OnStartServer();
            RandomisePowerup();
        }

        [Server]
        private void RandomisePowerup()
        {
            if (ActiveData.Instance.Powerups.Count <= 0) return;
            var i = Random.Range(0, ActiveData.Instance.Powerups.Count - 1);
            containedPowerup = ActiveData.Instance.Powerups[i];
            icon.sprite = containedPowerup.icon;
            RandomisePowerupRpc(i);
        }

        [ClientRpc]
        private void RandomisePowerupRpc(int i)
        {
            icon.sprite = ActiveData.Instance.Powerups[i].icon;
        }

        [Server]
        void OnTriggerEnter(Collider other)
        {
            if (Disabled) return;
            if (other.attachedRigidbody.TryGetComponent(out Ball ball)) if (ball.TryGetComponent(out LocalPowerupManager manager))
                {
                    if (manager.selectedPowerup) return;
                    Disabled = true;
                    PowerupRpc(false);
                    containedPowerup.manager = manager;
                    manager.PickedUp(containedPowerup);
                    Invoke(nameof(OnReset), respawnTime);
                }
        }

        [ClientRpc(includeOwner = true)]
        public void PowerupRpc(bool enabled)
        {
            Renderer?.SetActive(enabled);
            Particles?.Play();
            if(!enabled) OnPickup?.Invoke();
            else OnRespawn?.Invoke();
        }

        private void OnReset()
        {
            Disabled = false;
            PowerupRpc(true);
        }

    }
}
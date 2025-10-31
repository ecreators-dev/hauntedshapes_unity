using System;
using System.Linq;

using UnityEngine;

namespace Game.Interaction
{
    [RequireComponent(typeof(Collider))]
    public sealed class InteractionTrigger : MonoBehaviour, IInteractionZone
    {
        public event System.Action<Collider> TriggerEnterEvent;
        public event System.Action<Collider> TriggerExitEvent;

        [SerializeField] private Collider effectCollider;

        private CrosshairTargetBehaviour[] allReceivers;

        public bool IsPlayerInZoneThisFrame { get; private set; }

        private void Awake()
        {
            effectCollider.isTrigger = true;
        }

        private void Start()
        {
            // finde die erste Crosshair Interaction und speichere sie
            allReceivers = GetComponentsInParent<CrosshairTargetBehaviour>();
        }

        public ICrosshairSelectableObject[] GetReferences() => this.allReceivers.ToList().ToArray();

        /// <summary>
        /// Wird aufgerufen, wenn dieser Collider ein Trigger-Event auslöst "Enter"
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            // extension
            bool allowEvents = this.GameController_OnInteractionTriggerEnter(other).GetValueOrDefault();

            if (allowEvents)
                TriggerEnterEvent?.Invoke(other);

            IsPlayerInZoneThisFrame = other.CompareTag("Player");
            print($"Triggerzone Enter with: {other.tag}");
        }

        /// <summary>
        /// Wird aufgerufen, wenn dieser Collider ein Trigger-Event auslöst "Exit"
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            // extension
            bool allowEvents = this.GameController_OnInteractionTriggerExit(other).GetValueOrDefault();

            if (allowEvents)
                TriggerExitEvent?.Invoke(other);

            IsPlayerInZoneThisFrame = false;
        }
    }
}

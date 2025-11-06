using System;

using UnityEngine;

namespace Game.Interaction
{
    public interface IInteractionZone
    {
        event Action<Collider> TriggerEnterEvent;
        event Action<Collider> TriggerExitEvent;
        
        bool IsPlayerInZoneThisFrame { get; }

        ICrosshairSelectableObject[] GetReferences();
    }
}
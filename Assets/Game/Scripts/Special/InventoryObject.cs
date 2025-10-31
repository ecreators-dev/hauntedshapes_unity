using UnityEngine;

namespace Game.Interaction
{
    /// <summary>
    /// Beschreibt ein Objekt, was man für ein Inventar aufheben kann. Hiermit kann es über
    /// den Crosshair gefunden werden. Siehe Interface an der Klasse.
    /// </summary>
    public abstract class InventoryObject : CrosshairTargetBehaviour, IInteractiveObject
    {
        [SerializeField] private InteractionTrigger triggerZone;

        public IInteractionZone GetInteractionArea() => this.triggerZone;

        internal abstract InventoryAsset GetInventoryAsset();
    }
}

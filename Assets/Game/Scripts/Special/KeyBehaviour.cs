using UnityEngine;

namespace Game.Interaction
{
    /// <summary>
    /// Describes a Key Object (Schlüssel für ein Schloss/Tür etc.)
    /// </summary>
    public sealed class KeyBehaviour : InventoryObject
    {
        [SerializeField] private InventoryAsset Asset;

        public override bool CanSelect { get; internal protected set; } = true;
        public override bool IsLocked { get; } = false;

        internal override InventoryAsset GetInventoryAsset() => Asset;
    }
}

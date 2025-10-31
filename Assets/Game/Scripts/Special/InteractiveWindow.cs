using UnityEngine;

namespace Game.Interaction
{
    /// <summary>
    /// Ein Fenster-MonoBehaviour
    /// </summary>
    public class InteractiveWindow : CrosshairTargetBehaviour, IInteractiveObject
    {
        [SerializeField] private InteractionTrigger triggerZone;

        public override bool CanSelect { get; protected internal set; } = true;
        public override bool IsLocked { get; } = false;

        public IInteractionZone GetInteractionArea() => triggerZone;
    }
}

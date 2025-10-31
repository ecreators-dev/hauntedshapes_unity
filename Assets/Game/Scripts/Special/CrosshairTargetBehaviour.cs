using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Game.Interaction
{
    public abstract class CrosshairTargetBehaviour : MonoBehaviour, ICrosshairSelectableObject
    {
        [Tooltip("Text, der im Crosshair-Fokus angezeigt werden soll")]
        [SerializeField] protected string crosshairText = "Platzhalter";
        private List<IPlayerActor> zonePlayers;

        public abstract bool CanSelect { get; protected internal set; }
        public abstract bool IsLocked { get; }

        public string GetText() => string.IsNullOrEmpty(this.crosshairText) ? "<!>" : this.crosshairText;

        protected bool IsPlayerInZone()
        {
            return zonePlayers.Any();
        }

        internal void OnPlayerEnterTriggerZone(IPlayerActor player)
        {
            this.zonePlayers ??= new List<IPlayerActor>();
            this.zonePlayers.Add(player);
        }

        internal void OnPlayerExitTriggerZone(IPlayerActor player)
        {
            this.zonePlayers ??= new List<IPlayerActor>();
            this.zonePlayers.Remove(player);
        }
    }
}

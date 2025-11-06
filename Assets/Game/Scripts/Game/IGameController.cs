using Game.Interaction;

using UnityEngine;

namespace Game
{
    public interface IGameController
    {
        /// <summary>
        /// Eine TriggerZone meldet, dass sie betreten wurde
        /// </summary>
        bool InteractEnter(IInteractionZone zone, Collider enter);
        
        /// <summary>
        /// Prüft, ob für die Interaction die Bedingung getroffen ist
        /// </summary>
        bool IsConditionHit(InteractionConditionAsset unlockCondition, IActorBehaviour actor);
        
        /// <summary>
        /// Crosshair Fokus: Prüft, ob der Crosshair trifft. Damit ist nicht gemeint, wass es auslösen darf.
        /// </summary>
        bool IsCrosshairOnFocus(IInteractiveObject sender);
        
        /// <summary>
        /// Crosshair Fokus, prüft ob ein Bestimmter Spieler im Fokusbereich ist (TriggerZone)
        /// </summary>
        (bool inRange, bool hasRange) IsPlayerInRange(IInteractiveObject sender, int playerId);
        void PlayGame();

        /// <summary>
        /// Trägt ein Item im Visier ins Inventar ein
        /// </summary>
        void PutItemToInventory(PlayerBehaviour player);

        /// <summary>
        /// Liefert Informationen für die Erlaubnis einer Interaktion
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        (bool inTrigger, bool inRange, bool leftMouseUpFrame, bool shouldInteract) ShouldInteract(IInteractiveObject sender);
    }
}
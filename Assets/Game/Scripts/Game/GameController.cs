#nullable enable
using Game.Interaction;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameController : MonoBehaviour, IGameLoadProgress, IGameController
    {
        /// <summary>
        /// Eine ID, die alle Spieler, bzw. einen beliebigen Spieler abbildet
        /// </summary>
        private const int PLAYER_ID_ANY = 0;
        private PlayerBehaviour? Player;
        private Dictionary<int, (IPlayerActor player, IInteractionZone trigger)>? triggerZonePlayers;
        private List<IInteractiveObject> interactiveObjects;

        [SerializeField] private InputActionReference interactButton;

        [SerializeField] private int sceneIndexDebug;
        private static GameController? instance;

        internal static GameController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<GameController>();
                }

                return instance ?? throw new System.Exception("Kein Controller!");
            }
        }

        private
#if !UNITY_EDITOR
            async
#endif
            void Start()
        {
            interactiveObjects = FindObjectsByType<CrosshairTargetBehaviour>(FindObjectsSortMode.None).Cast<IInteractiveObject>().ToList();
            interactiveObjects ??= new List<IInteractiveObject>();

            // für Debug wird die Map01 geladen. Später natürlich das Menü
#if !UNITY_EDITOR
            await SceneManager.LoadSceneAsync(sceneIndexDebug, LoadSceneMode.Additive);
#endif
        }

        /// <summary>
        /// Verwaltet die Aufnahme eines Gegenstands in das Spieler-Inventars
        /// </summary>
        public void PutItemToInventory(PlayerBehaviour player)
        {
            ICrosshairSelectableObject? select = ICrosshairSelector.GetInstance().FindInSelection<InventoryObject>();
            if (select is null)
            {
                return;
            }

            if (player.GetInventory() is IInventorySource inventory)
            {
                InventoryObject objectToInsert = (InventoryObject)select;

                InventoryAsset insert = objectToInsert.GetInventoryAsset();
                if (insert == null)
                {
                    throw new System.Exception($"Fehlendes Asset an {nameof(InventoryObject)}");
                }

                inventory.AddItem(insert);
            }
        }

        /// <summary>
        /// Prüft, ob Spieler die Bedingung erfüllt
        /// </summary>
        public bool IsConditionHit(InteractionConditionAsset unlockCondition, IActorBehaviour actor)
        {
            return Instance?.IsValidCondition(actor, unlockCondition) ?? false;
        }

        private bool IsValidCondition(IActorBehaviour actor, InteractionConditionAsset unlockCondition)
        {
            if (unlockCondition == null)
            {
                return true;
            }

            bool valid = true;
            if (unlockCondition.PlayerLevel > 0 && Player.Data.PlayerLevel < unlockCondition.PlayerLevel)
            {
                valid = false;
            }
            if (unlockCondition.MustBeGhost && !actor.IsGhost())
            {
                valid = false;
            }
            if (unlockCondition.KeyValue != null && !HasKey(actor, unlockCondition.KeyValue))
            {
                valid = false;
            }
            return valid;
        }

        private bool HasKey(IActorBehaviour actor, KeyId keyValue)
        {
            return actor is IPlayerActor player
                && player.GetInventory() is IInventorySource inventory
                && inventory.HasKey(keyValue);
        }

        private void FixedUpdate()
        {
            ShowCrosshairTargetText();
        }

        private void ShowCrosshairTargetText()
        {
            ICrosshairSelectableObject? select = ICrosshairSelector.GetInstance().FindInSelection<CrosshairTargetBehaviour>();
            if (select is null)
            {
                ICrosshairSelector.GetInstance().HideText();
                return;
            }

            (bool inRange, bool hasZone) interactive = default;
            if (select is IInteractiveObject interact)
            {
                interactive = IsPlayerInRange(interact, PLAYER_ID_ANY);
            }

            ICrosshairSelector.GetInstance().ShowTarget(select, !interactive.hasZone || interactive.inRange);
        }

        public bool InteractEnter(IInteractionZone zone, Collider enter)
        {
            if (enter.TryGetComponent(out PlayerBehaviour player))
            {
                int playerId = GetPlayerId(player);
                OnPlayerEnterTriggerZone(zone, playerId, player);
            }
            return true;
        }

        internal bool InteractExit(IInteractionZone zone, Collider enter)
        {
            if (enter.TryGetComponent(out PlayerBehaviour player))
            {
                int playerId = GetPlayerId(player);
                OnPlayerExitTriggerZone(zone, playerId, player);
            }
            return true;
        }

        private static int GetPlayerId(PlayerBehaviour player) => player.Data.Id;

        private void EnsureTriggerZoneCache()
        {
            this.triggerZonePlayers ??= new Dictionary<int, (IPlayerActor player, IInteractionZone trigger)>();
        }

        /// <summary>
        /// Kurz gesagt: Ob der Spieler mit dem Objekt interagieren darf und möchte <br/>
        /// Ermittelt, ob das Objekt mit dessen TriggerZone in Reichweite ist und aktiv gerade die Linke Maustaste losgelassen wurde.
        /// </summary>
        public (bool inTrigger, bool inRange, bool leftMouseUpFrame, bool shouldInteract) ShouldInteract(IInteractiveObject sender)
        {
            // teste crosshair
            // teste zone "trigger"
            (bool zoneTrigger, bool hasZone) = IsPlayerInRange(sender, PLAYER_ID_ANY);
            bool crosshairHit = IsCrosshairOnFocus(sender);
            bool keyPressed = interactButton != null ? interactButton.action.WasPressedThisFrame() : Mouse.current.leftButton.wasReleasedThisFrame;
            return (zoneTrigger, crosshairHit, keyPressed, (!hasZone || zoneTrigger) && crosshairHit && keyPressed);
        }

        /// <summary>
        /// Bestimmt, ob das Objekt im Crosshair-Fokus ist
        /// </summary>
        public bool IsCrosshairOnFocus(IInteractiveObject sender)
        {
            ICrosshairSelectableObject? selectObjects = ICrosshairSelector.GetInstance().FindInSelection<CrosshairTargetBehaviour>();
            return selectObjects == sender;
        }

        /// <summary>
        /// Prüft, ob ein Spieler in der TriggerZone steht. Verwende playerId=0, um einen beliebigen Spieler in der Zone zu finden.
        /// Achte darauf, dass der Sender eine <see cref="IInteractionZone"/> in <see cref="IInteractiveObject.GetInteractionArea"/> liefert.
        /// </summary>
        public (bool inRange, bool hasRange) IsPlayerInRange(IInteractiveObject sender, int playerId)
        {
            bool hasZone = false;
            var zone = sender.GetInteractionArea();
            if (zone == null)
            {
                hasZone = false;
                return (true, hasZone);
            }

            hasZone = true;
            if (zone.IsPlayerInZoneThisFrame)
            {
                EnsureTriggerZoneCache();
                if (playerId != PLAYER_ID_ANY && triggerZonePlayers!.TryGetValue(playerId, out (IPlayerActor player, IInteractionZone trigger) info))
                {
                    return (info.trigger == zone, hasZone);
                }
            }
            return (true, hasZone);
        }

        private void OnPlayerEnterTriggerZone(IInteractionZone zone, int playerId, IPlayerActor player)
        {
            EnsureTriggerZoneCache();
            this.triggerZonePlayers[playerId] = (player, zone);

            // finde das/die Objekt(e) mit der zone
            bool foundObj = false;
            List<IInteractiveObject> matchingObj = interactiveObjects.Where(obj => obj.GetInteractionArea() == zone).ToList();
            foreach (IInteractiveObject crosshairTarget in matchingObj)
            {
                if (crosshairTarget is CrosshairTargetBehaviour receiver)
                {
                    foundObj = true;
                    receiver.OnPlayerEnterTriggerZone(player);
                }
            }

            if (!foundObj)
            {
                Trace.TraceWarning("Keinen Empfänger gefunden");
            }
        }


        private void OnPlayerExitTriggerZone(IInteractionZone zone, int playerId, IPlayerActor player)
        {
            EnsureTriggerZoneCache();
            this.triggerZonePlayers.Remove(playerId);

            // finde das/die Objekt(e) mit der zone
            bool foundObj = false;
            List<IInteractiveObject> matchingObj = interactiveObjects.Where(obj => obj.GetInteractionArea() == zone).ToList();
            foreach (IInteractiveObject crosshairTarget in matchingObj)
            {
                if (crosshairTarget is CrosshairTargetBehaviour receiver)
                {
                    foundObj = true;
                    receiver.OnPlayerExitTriggerZone(player);
                }
            }

            if (!foundObj)
            {
                Trace.TraceWarning("Keinen Empfänger gefunden");
            }
        }
    }
}
#nullable restore
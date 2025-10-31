using Game.Audio;

using System;
using System.Collections;

using UnityEngine;

using static Game.Interaction.TimelineObjectOpenClose;

namespace Game.Interaction
{
    /// <summary>
    /// Controls door behaviours
    /// </summary>
    [RequireComponent(typeof(TimelineObjectOpenCloseGroup))]
    public class InteractiveDoor : CrosshairTargetBehaviour, IInteractiveObject, IGameLoadProgress
    {
        [Tooltip("Legt die Sekunden fest, die es nach dem Start von Klinke dauert bis diese wieder in seine Ausgangsposition zurück geht")]
        [Min(0)]
        [SerializeField] private float openDelayHandleUp = 0.3f;

        [Tooltip("Legt die Sekunden fest, die es nach dem Start von Tür dauert bis diese ins Schloss fällt und einen Sound spielt")]
        [Min(0)] // Dauer Tür Schließen (1.3 Sekunden) minus Dauer Sound/Klinke-Animation (0.3 Sekunden) = 1s
        [SerializeField] private float closeSoundDelay = 1f;

        [SerializeField] private TimelineObjectOpenClose handleController;
        [SerializeField] private TimelineObjectOpenClose doorBoardController;
        [SerializeField] private RandomAudioClipPlayer audioPlayer;

        [Tooltip("Tür-Status bei Spielstart und nach Aktion")]
        [SerializeField] private OpenCloseEnum openState = OpenCloseEnum.Close;
        private OpenCloseEnum openStateBefore = OpenCloseEnum.Open;

        // Schlüssel oder andere Bedingungen, um zu öffnen müssen gegeben sein, wenn dieses Feld nicht leer bleibt
        [Header("Bedingungen zum Öffnen u. Schließen")]
        [Tooltip("(Optional) Bedingung für das Öffnen")]
        [SerializeField] private InteractionConditionAsset unlockCondition;
        [SerializeField] private InteractionTrigger triggerZone;

        [Tooltip("Das Spiel schaltet diese Tür Interaktionen frei, wenn =checked=")]
        [SerializeField] private bool isLocked; // wenn gesetzt, dann muss es erst freigeschaltet werden
        [SerializeField] private bool RevokeUnlockOnClose; // wenn gesetzt und eine Bedingung vorliegt, muss diese nach dem erneutem Schließen wieder erfüllt werden

        private TimelineObjectOpenCloseGroup controller;

        public override bool CanSelect { get; protected internal set; } = true;
        public override bool IsLocked { get => this.isLocked; }

        private void Awake()
        {
            this.controller = GetComponent<TimelineObjectOpenCloseGroup>();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (openState != openStateBefore)
            {
                ShowDoorState();
            }
            openStateBefore = openState;
        }
#endif

        private void ShowDoorState()
        {
            if (openState == OpenCloseEnum.Open)
            {
                OpenDoor(null);
            }
            else
            {
                CloseDoor();
            }
        }

        private void Start()
        {
            audioPlayer.SetMute(true);
            ShowDoorState();
        }

        public void Unlock(GameActorBehaviour sender)
        {
            if (IsLocked && unlockCondition != null && this.GetGameController().IsConditionHit(unlockCondition, sender))
            {
                isLocked = false;
            }
        }

        [ContextMenu("Open Door")]
        private void OpenDoor()
        {
            if (!Application.isPlaying)
                return;

            openState = OpenCloseEnum.Open;
            openStateBefore = OpenCloseEnum.Close;

            if (RevokeUnlockOnClose && unlockCondition != null)
            {
                isLocked = true;
            }

            StartCoroutine(OpenSpecial());

            IEnumerator OpenSpecial()
            {
                // Klinke und Türbrett
                controller.Open();

                // klinke wieder hoch, auch beim öffnen!
                yield return new WaitForSeconds(openDelayHandleUp);

                // nur Klinke
                handleController.Close();
                audioPlayer.SetMute(false);
                yield return null;
            }
        }

        public void OpenDoor(GameActorBehaviour actor)
        {
            if (!ConditionMatching(actor))
            {
                audioPlayer.SetMute(false);
                return;
            }

            EnsureConsumeKey(actor);
            OpenDoor();
        }

        [ContextMenu("Close Door")]
        public void CloseDoor()
        {
            if (!Application.isPlaying)
                return;

            openState = OpenCloseEnum.Close;
            openStateBefore = OpenCloseEnum.Open;

            StartCoroutine(CloseSpecial());

            IEnumerator CloseSpecial()
            {
                // nur Türbrett schließen, keine Klinke
                doorBoardController.Close();

                // Audioclip abspielen für die Klinke, auch wenn diese sich nicht bewegt!
                // "ins Schloss fallen" Sound
                yield return new WaitForSeconds(closeSoundDelay);
                // Fällt ins Schloss Sound! keine Klinkenbewegung

                audioPlayer.Play();
                audioPlayer.SetMute(false);

                yield return null;
            }
        }

        private void EnsureConsumeKey(GameActorBehaviour actor)
        {
            if (unlockCondition == null)
                return;

            if (unlockCondition.KeyValue == null)
                return;

            if (actor is not IPlayerActor player)
                return;

            if (player.GetInventory() is not IInventorySource inventory)
                return;

            inventory.ConsumeKey(unlockCondition.KeyValue);
        }

        private bool ConditionMatching(IActorBehaviour actor)
        {
            if (Application.isPlaying)
            {
                if (IsLocked)
                    return false;

                return this.GetGameController().IsConditionHit(unlockCondition, actor);
            }

            return true;
        }

        public IInteractionZone GetInteractionArea()
        {
            if (triggerZone == null)
                print("Fehlende Triggerzone");

            return triggerZone;
        }
    }
}

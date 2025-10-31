using System;

using UnityEngine;
using UnityEngine.Playables;

namespace Game.Interaction
{
    public class TimelineObjectOpenClose : MonoBehaviour, ITimelineOpenCloseBehaviour, IGameLoadProgress
    {
        // playdirector
        // animationsnamen Groupnames
        // open  /  close
        // Auslöser
        [SerializeField] private PlayableDirector director;

        [Tooltip("Name der Track-Group im Timeline Editor")]
        [SerializeField] private string groupNameOpen;

        [Tooltip("Name der Track-Group im Timeline Editor")]
        [SerializeField] private string groupNameClose;


        private OpenCloseEnum animationTypeBefore = OpenCloseEnum.Close;
        private TimelinePlayer timelineDirector;

        [ContextMenu("Open Object")]
        public void Open() => RunAnimation(OpenCloseEnum.Open);

        [ContextMenu("Close Object")]
        public void Close() => RunAnimation(OpenCloseEnum.Close);

        private void Start()
        {
            this.timelineDirector = new TimelinePlayer(director);
        }

        private void RunAnimation(OpenCloseEnum behaviour)
        {
            if (behaviour == this.animationTypeBefore)
                return;

            this.animationTypeBefore = behaviour;

            // nur debug: kann gelöscht werden
#if UNITY_EDITOR
            if (!Application.isPlaying)
                this.timelineDirector = null;
            this.timelineDirector ??= new TimelinePlayer(director);
#endif

            // behalten ab hier!
            string groupName = GetGroupNameByBehaviour(behaviour);
            timelineDirector.PlayGroup(groupName);
        }

        private string GetGroupNameByBehaviour(OpenCloseEnum behaviour)
        {
            return behaviour == OpenCloseEnum.Open ? groupNameOpen : groupNameClose;
        }

        public void ResetStatus()
        {
            // bei mehr als 2 Enums einen anderen wählen, als den Aktiven
            this.animationTypeBefore = OpenCloseEnum.Close;
        }

        public enum OpenCloseEnum
        {
            Close,
            Open
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Game.Interaction
{
    public class TimelineObjectOpenCloseGroup : MonoBehaviour, ITimelineOpenCloseBehaviour, ITimelineOpenCloseIndexBehaviour, IGameLoadProgress
    {
        [SerializeField] private List<TimelineObjectOpenClose> controlables;
        [SerializeField] private List<IndexDelay> delays;

        [ContextMenu("Open Objects...")]
        public void Open()
        {
            if (controlables == null)
                return;

            for (int i = 0; i < controlables.Count; i++)
            {
                TimelineObjectOpenClose animation = controlables[i];
                StartOpenAnimation(i, animation);
            }
        }

        [ContextMenu("Close Objects...")]
        public void Close()
        {
            if (controlables == null)
                return;

            for (int i = 0; i < controlables.Count; i++)
            {
                TimelineObjectOpenClose animation = controlables[i];
                StartCloseAnimation(i, animation);
            }
        }

        private void StartOpenAnimation(int index, ITimelineOpenCloseBehaviour target)
        {
            StartCoroutine(WaitAndPlay(index, target.Open));

        }

        private void StartCloseAnimation(int index, ITimelineOpenCloseBehaviour target)
        {
            StartCoroutine(WaitAndPlay(index, target.Close));
        }

        private IEnumerator WaitAndPlay(int index, Action targetCall)
        {
            // warte die Sekunden ab, wenn eingestellt ist, dass vor der Aktion
            // eine Wartezeit sein soll
            IndexDelay? delay = delays.FirstOrDefault(d => d.ObjectIndex == index);
            if (delay != null)
                yield return new WaitForSecondsRealtime(delay.DelaySeconds);

            targetCall.Invoke();
            yield return null;
        }

        public void Close(int playableIndex)
        {
            if (controlables != null && playableIndex < controlables.Count)
            {
                controlables[playableIndex].Close();
            }
        }

        public void Open(int playableIndex)
        {
            if (controlables != null && playableIndex < controlables.Count)
            {
                controlables[playableIndex].Open();
            }
        }

        public void ResetStatus()
        {
            foreach (TimelineObjectOpenClose animation in controlables)
            {
                animation.ResetStatus();
            }
        }
    }
}

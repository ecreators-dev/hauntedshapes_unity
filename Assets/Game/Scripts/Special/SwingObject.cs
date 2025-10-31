using UnityEngine;

namespace Game.Interaction
{
    public sealed class SwingObject : MonoBehaviour, ISwingPush, IGameLoadProgress
    {
        [SerializeField] private SwingSeatRigidbody leftSeat;
        [SerializeField] private SwingSeatRigidbody rightSeat;

        [Tooltip("Überschreibt random die Seite, die angeschubst wird")]
        [SerializeField] public bool RandomPushSide = false;

        public bool PushSideRight { get; set; } = false;

        public DurationSeconds Push(SwingEffect option)
        {
            if (RandomIsRightSwing())
            {
                PushRight(option);
            }
            else
            {
                PushLeft(option);
            }
            return -1f;
        }

        private bool RandomIsRightSwing()
        {
            bool randomLeft = RandomPushSide && Random.value * 100 < 50;
            bool pushRight = RandomPushSide && !randomLeft || !RandomPushSide && PushSideRight;
            return pushRight;
        }

        public void PushLeft(SwingEffect option)
        {
            leftSeat.Push(option);
        }

        public void PushRight(SwingEffect option)
        {
            rightSeat.Push(option);
        }

        [ContextMenu("Anschubsen (Zufall)")]
        public float StartPushRandom()
        {
            bool oldState = RandomPushSide;
            RandomPushSide = true;
            try
            {
                if (RandomIsRightSwing())
                {
                    return rightSeat.StartPushRandom();
                }

                return leftSeat.StartPushRandom();
            }
            finally
            {
                RandomPushSide = oldState;
            }
        }

        [ContextMenu("Anschubsen Links")]
        public void StartPushRandomLeft()
        {
            leftSeat.StartPushRandom();
        }

        [ContextMenu("Anschubsen Rechts")]
        public void StartPushRandomRight()
        {
            rightSeat.StartPushRandom();
        }
    }
}

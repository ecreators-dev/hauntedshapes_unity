using System;

using UnityEngine;

namespace Game.Interaction
{
    public sealed class LocaleYShifter : MonoBehaviour, IHandAction
    {
        [Min(0)]
        [SerializeField] private float yOffset = 0.33f;

        [Min(0)]
        [SerializeField] private float tempoSeconds = 0.4f;
        [SerializeField] private bool isLifted = false;

        private Transform myTransform;
        private Vector3 handDownPositionLocal;
        private Vector3 targetPosition;

        private void Awake()
        {
            myTransform = transform;
            handDownPositionLocal = myTransform.localPosition;
        }

        private void Start()
        {
            if (isLifted)
            {
                HandUp();
            }
            else
            {
                HandDown();
            }
        }

        public void HandUp()
        {
            targetPosition = GetLiftedPosition();
            isLifted = true;
        }

        private Vector3 GetLiftedPosition()
        {
            return transform.localPosition + transform.up * yOffset;
        }

        public void HandDown()
        {
            targetPosition = handDownPositionLocal;
            isLifted = false;
        }

        private void Update()
        {
            if ((targetPosition - myTransform.localPosition).sqrMagnitude > 0)
            {
                myTransform.localPosition = Vector3.Lerp(myTransform.localPosition, targetPosition, Time.deltaTime * tempoSeconds);
            }
        }

        public bool IsUp() => this.isLifted;
    }
}

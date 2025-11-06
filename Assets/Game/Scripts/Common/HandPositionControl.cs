#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Interaction
{
    public class HandPositionControl : MonoBehaviour
    {
        [SerializeField] private LocaleYShifter? leftHand;
        [SerializeField] private LocaleYShifter? rightHand;

        private bool leftUp;
        private bool rightUp;
        [SerializeField] private InputActionReference? liftLeftHandControl;
        [SerializeField] private InputActionReference? liftRightHandControl;

        private void OnEnable()
        {
            if (liftLeftHandControl != null)
                liftLeftHandControl.action.Enable();

            if (liftRightHandControl != null)
                liftRightHandControl.action.Enable();
        }

        private void Start()
        {
            if (leftHand != null && liftLeftHandControl != null)
                leftUp = leftHand.IsUp();

            if (rightHand != null && liftRightHandControl != null)
                rightUp = rightHand.IsUp();
        }

        private void Update()
        {
            HandleLeftHand();
            HandleRightHand();
        }

        private void HandleRightHand() => HandleHand(liftRightHandControl, ref rightUp, rightHand, HandUp);

        private void HandleLeftHand() => HandleHand(liftLeftHandControl, ref leftUp, leftHand, HandUp);

        private static void HandleHand(InputActionReference? control, ref bool state, LocaleYShifter? hand, System.Action<LocaleYShifter, bool> operatorAction)
        {
            if (control != null)
            {
                bool wasUp = state;
                state = control.action.IsPressed();
                if (state != wasUp)
                {
                    operatorAction.Invoke(hand!, state);
                }
            }
        }

        public void HandUp(LocaleYShifter? hand, bool up)
        {
            if (hand == null)
                return;

            if (up)
                hand.HandUp();
            else
                hand.HandDown();
        }
    }
}
#nullable restore
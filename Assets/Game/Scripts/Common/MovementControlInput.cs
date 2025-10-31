using System.Collections;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Interaction.Input
{
    [RequireComponent(typeof(CharacterController))] // Script Execution Order: 2
    public sealed class MovementControlInput : MonoBehaviour, IGameLoadProgress
    {
        [Header("Timing & Speed Preset")]
        [SerializeField] private MovementTimingSettingsRef timingNormal;
        [SerializeField] private MovementTimingSettingsRef timingCrouch;

        [Header("Gravity")]
        [Min(0.1f)][SerializeField] private float gravity = 9.81f;

        [Header("Ground Check")]
        [Range(0.05f, 1.0f)][SerializeField] private float groundCheckDistance = 0.3f;
        [SerializeField] private LayerMask groundMask = ~0;

        [Header("Crouch")]
        [Min(0.1f)][SerializeField] private float standHeight = 2.0f;
        [Min(0.1f)][SerializeField] private float crouchHeight = 2.0f;
        [Min(0.01f)][SerializeField] private float crouchTransition = 0.15f;
        [SerializeField] private InputActionReference crouchControl;

        private CharacterController controller;
        private Transform cachedTransform;
        private float initialHeight;
        private Vector3 initialCenter;
        private Vector2 moveInput;
        private Vector3 currentVelocity;
        private float verticalVelocity;
        private bool isGrounded;
        private bool crouching;
        private float crouchVelocityRef;
        private bool hasInitialGroundFix;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            cachedTransform = transform;
            initialHeight = controller.height;
            initialCenter = controller.center;
        }

        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        public void OnCrouch(InputValue value)
        {
            crouching = value.isPressed;
            print("Crouch: true/false");
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.performed) // Taste wurde gedrückt
            {
                crouching = true;
                // Hier den Crouch-Status aktivieren, z.B. die Höhe anpassen
                print("Crouch: true");
            }
            else if (context.canceled) // Taste wurde losgelassen
            {
                crouching = false;
                // Hier den Crouch-Status wieder zurücksetzen
                print("Crouch: false");
            }
        }

        private void Update()
        {
            if (!hasInitialGroundFix)
            {
                FixInitialGroundOverlap();
                hasInitialGroundFix = true;
            }

            GroundCheck();

            MovementTimingSettings preset = (crouching ? (timingCrouch ?? timingNormal) : timingNormal).asset;
            float maxSpeed = preset.maxSpeed;
            if (crouching)
                maxSpeed *= 0.65f;

            float deltaTime = Time.deltaTime;
            Vector3 targetVelocity =
                (cachedTransform.right * moveInput.x + cachedTransform.forward * moveInput.y) * maxSpeed;

            if (moveInput.sqrMagnitude > 0.01f)
            {
                float accelRate = maxSpeed / Mathf.Max(0.001f, preset.accelerationTime);
                currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, accelRate * deltaTime);
            }
            else
            {
                float decelRate = maxSpeed / Mathf.Max(0.001f, preset.decelerationTime);
                currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, decelRate * deltaTime);
            }

            if (isGrounded && verticalVelocity < 0f)
                verticalVelocity = -gravity * 2f * deltaTime; // ✅ FIX #1
            else
                verticalVelocity -= gravity * deltaTime;

            Vector3 motion = currentVelocity;
            motion.y = verticalVelocity;
            controller.Move(motion * deltaTime);

            if (isGrounded)
                controller.Move(Vector3.down * gravity * deltaTime * 0.15f); // ✅ FIX #2

            // Verhältnis der neuen Höhe zur alten Höhe
            float targetHeight = crouching ? crouchHeight : standHeight;
            float heightRatio = targetHeight / initialHeight;

            // Center anpassen, damit Füße fix bleiben
            controller.height = Mathf.SmoothDamp(controller.height, targetHeight, ref crouchVelocityRef, crouchTransition);
            controller.center = new Vector3(
                initialCenter.x,
                initialCenter.y * heightRatio,
                initialCenter.z
            );

            if (crouching && crouchControl != null && !crouchControl.action.IsPressed())
            {
                crouching = false;
            }
        }

        /// <summary>
        /// Korrigiert initiales Einsinken NUR einmal.
        /// </summary>
        private void FixInitialGroundOverlap()
        {
            const float step = 0.05f;
            const int maxAttempts = 20;

            for (int i = 0; i < maxAttempts; i++)
            {
                bool overlaps = Physics.CheckCapsule(
                    cachedTransform.position + Vector3.up * controller.radius,
                    cachedTransform.position + Vector3.up * (controller.height - controller.radius),
                    controller.radius,
                    groundMask,
                    QueryTriggerInteraction.Ignore
                );

                if (!overlaps)
                    break;

                cachedTransform.position += Vector3.up * step;
            }
        }

        private void GroundCheck()
        {
            Vector3 origin = cachedTransform.position + Vector3.up * 0.05f;
            float radius = controller.radius * 0.9f;

            isGrounded = Physics.SphereCast(
                origin,
                radius,
                Vector3.down,
                out _,
                groundCheckDistance,
                groundMask,
                QueryTriggerInteraction.Ignore
            ); // ✅ FIX #4 – stabil, auch an Kanten
        }
    }
}

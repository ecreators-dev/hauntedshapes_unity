using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Interaction.Input
{
    /// <summary>
    /// Steuert die Kameraansicht (Mausrotation) des Spielers.
    /// </summary>
    public sealed class ViewControlInput : MonoBehaviour, IGameLoadProgress
    {
        [Header("Timing Preset")]
        [SerializeField] private MovementTimingSettingsRef timingPreset;

        [Header("Mouse")]
        [SerializeField] private float mouseSensitivity = 1.2f;
        [SerializeField] private Transform playerBody;
        [SerializeField] private Key toggleViewActive = Key.Tab;

        private Transform cachedTransform;
        private Vector2 lookInput;
        private Vector2 lookVelocity;
        private float pitch;
        private bool offline;

        private void Awake()
        {
            cachedTransform = transform; // ✅ local cache
        }

        private void Start()
        {
            CursorLocked();
        }

        private static void CursorLocked()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private static void CursorUnlocked()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void OnLook(InputValue value)
        {
            lookInput = value.Get<Vector2>();
        }

        private void Update()
        {
            if (toggleViewActive.WasKeyReleasedThisFrame())
            {
                offline = !offline;
                if (offline)
                {
                    CursorUnlocked();
                }
                else
                {
                    CursorLocked();
                }
            }

            if (offline)
            {
                return;
            }

            float dt = Time.deltaTime;

            Vector2 target = lookInput * mouseSensitivity;
            bool userMovingCamera = lookInput.sqrMagnitude > 0.001f;

            float accelT = Mathf.Max(0.001f, timingPreset.asset.accelerationTime);
            float decelT = Mathf.Max(0.001f, timingPreset.asset.decelerationTime);

            float smoothingSpeed = userMovingCamera ?
                (1f / accelT) : (1f / decelT);

            lookVelocity = Vector2.MoveTowards(lookVelocity, target, smoothingSpeed * dt);

            // Apply Rotation
            float mouseX = lookVelocity.x;
            float mouseY = lookVelocity.y;

            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -85f, 85f);

            cachedTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}

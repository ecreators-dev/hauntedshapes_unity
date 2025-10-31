using UnityEngine;

namespace Game.Interaction.Input
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Movement Timing (Movement Setting)", menuName = "Haunted-Shapes/New Movement State")]
    public sealed class MovementTimingSettings : ScriptableObject
    {
        [Header("Time Settings")]
        [Min(0.01f)]
        [SerializeField] public float maxTime = 1.0f;

        [Tooltip("Zeit von Stillstand zu voller Geschwindigkeit")]
        [Min(0f)]
        [SerializeField] public float accelerationTime = 0.2f;

        [Tooltip("Zeit von voller Geschwindigkeit zu Stillstand")]
        [Min(0f)]
        [SerializeField] public float decelerationTime = 0.2f;

        [Header("Max Speed")]
        [Min(0f)]
        [SerializeField] public float maxSpeed = 4.5f;

        // Laufzeit-Helper
        public float TotalPhases => Mathf.Max(0.0001f, accelerationTime + decelerationTime);
        public float AccelRatio => accelerationTime / TotalPhases;
        public float DecelRatio => decelerationTime / TotalPhases;
    }
}

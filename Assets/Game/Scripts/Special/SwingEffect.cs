using UnityEngine;

using System;

namespace Game.Interaction
{
    [CreateAssetMenu(fileName = "Swing Effect", menuName = "Haunted-Shapes/Swing-Effect")]
    public sealed class SwingEffect : ScriptableObject
    {
        [SerializeField] internal PushTypeEnum Mode = PushTypeEnum.Push;

        [Range(-90f, 90f)]
        [SerializeField] public float StartAngle;

        [Header("Timing")]
        [Tooltip("Zeit in Sekunden, wie lange es Dauert, bis der Winkel erreicht wird")]
        [Min(0f)]
        [SerializeField] public float DragSeconds;

        [Tooltip("Zeit in Sekunden, wie lange der Winkel gehalten wird")]
        [Min(0f)]
        [SerializeField] public float HoldSeconds;

        [Header("(Darf negativ sein für Rückwärts)")]
        [SerializeField] public float PushForceAngular;

        /// <summary>
        /// Nicht in Inspector!
        /// </summary>
        public float TimeToDropSeconds => DragSeconds + HoldSeconds;

        /*
         Kombinationen
        Ergebnis        | PushForce                 | DragTime
        Anschieben      |   stärke                  |   0s
        Beschleunigen   |   stärke/frame ber Zeit   |   1s
        Umkehren        |   gegenwirkung            |   0s
        Vorbereiten     |   0                       |   1s
         */

        [System.Serializable]
        public enum PushTypeEnum
        {
            AngleAndDrop = 0,
            Push = 1
        }
    }
}

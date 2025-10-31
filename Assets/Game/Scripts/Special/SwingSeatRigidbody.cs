using System.Collections;

using UnityEngine;

using Random = UnityEngine.Random;
using UnityEngine.UIElements;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Interaction
{
    /// <summary>
    /// Definiert einen Sitz einer Schaukel (Swing) um diese mit Rigidbody und Hinge zum Schwingen zu bringen
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    //[RequireComponent(typeof(HingeJoint))]
    [RequireComponent(typeof(Collider))]
    public class SwingSeatRigidbody : MonoBehaviour, ISwingPush, IGameLoadProgress
    {
        // seconds
        private const int SWING_TIME_IDLE = 3;

        [InspectorName("0 Low")]
        [SerializeField] internal SwingEffect lowEffect;
        [InspectorName("1 Medium")]
        [SerializeField] internal SwingEffect mediumEffect;
        [InspectorName("2 Strong")]
        [SerializeField] internal SwingEffect strongEffect;
        [InspectorName("3 Extreme")]
        [SerializeField] internal SwingEffect extremeEffect;

        [Min(0)]
        [SerializeField] private float airResistance = 0.1f;
        [Min(0)]
        [SerializeField] private float rotationSlowdown = 2f;

        [Header("Start Fx (0 bis 2s nach Start)")]
        [SerializeField] private bool startRandom = false;
        private Coroutine routine;
        private Coroutine randomRoutine;

        private Rigidbody Rigidbody { get; set; }
        private HingeJoint JointAxis { get; set; }

        private void Awake()
        {
            this.Rigidbody = GetComponent<Rigidbody>();
            this.JointAxis = GetComponent<HingeJoint>();
        }

        private void Start()
        {
            EnableSwingAutomaticRandomly(true);
        }

        private void EnableSwingAutomaticRandomly(bool useRandomStartWaiting)
        {
            if (!startRandom)
                return;

            // random start in Sekunden
            float randomSeconds = GetRandomWait();
            if (randomRoutine != null)
                StopCoroutine(randomRoutine);
            randomRoutine = StartCoroutine(StartRandom());

            IEnumerator StartRandom()
            {
                while (startRandom)
                {
                    if (useRandomStartWaiting)
                        yield return new WaitForSecondsRealtime(randomSeconds);

                    float durationSeconds = StartPushRandom();

                    if (startRandom)
                    {
                        print("Random Swing will start");
                        yield return new WaitForSecondsRealtime(durationSeconds);

                        randomSeconds = GetRandomWait();
                        yield return null;
                    }
                }

                yield return null;
            }
        }

        /// <summary>
        /// Schubst die Schaukel einmal an mit zufälliger Stärke
        /// </summary>
        public float StartPushRandom()
        {
            SwingEffect[] effects = new SwingEffect[SWING_TIME_IDLE]
            {
                lowEffect, mediumEffect, strongEffect
            };

            // random effect
            SwingEffect selectedEffect = effects[Random.Range(0, SWING_TIME_IDLE)];
            float durationSeconds = Push(selectedEffect);
            return durationSeconds;
        }

        private static float GetRandomWait()
        {
            return Random.value * 2;
        }

        [ContextMenu("Schaukeln 0")]
        public void AnschaukelnLow() => Push(lowEffect);

        [ContextMenu("Schaukeln 1")]
        public void AnschaukelnMedium() => Push(mediumEffect);

        [ContextMenu("Schaukeln 2")]
        public void AnschaukelnStrong() => Push(strongEffect);

        [ContextMenu("Schaukeln 3")]
        public void Anschieben() => Push(extremeEffect);


        /// <summary>
        /// Startet einen Schaukeleffekt
        /// </summary>
        public DurationSeconds Push(SwingEffect option)
        {
            //this.Rigidbody.isKinematic = false;
            //this.Rigidbody.useGravity = true; // Muss an!
            //this.Rigidbody.linearDamping = airResistance; // Leichte Luftreibung
            //this.Rigidbody.angularDamping = rotationSlowdown; // Drehdämpfung – stoppt endloses Drehen

            if (option.Mode == SwingEffect.PushTypeEnum.Push)
            {
                //this.JointAxis.connectedBody.constraints = RigidbodyConstraints.FreezeAll | RigidbodyConstraints.FreezeRotationX;
                // sofort anschieben vorwärts oder rückwärts
                Push(option.PushForceAngular);
                return SWING_TIME_IDLE;
            }
            else
            {
                //this.JointAxis.connectedBody.constraints = RigidbodyConstraints.FreezeAll;
                SwingToAngle(option.StartAngle, option.TimeToDropSeconds, option.HoldSeconds);
                return option.TimeToDropSeconds + option.HoldSeconds + SWING_TIME_IDLE;
            }
        }

        private void SwingToAngle(float targetAngle, float moveTime, float holdTime, float motorForce = 100f)
        {
            targetAngle *= -1;

            if (routine != null)
                StopCoroutine(routine);
            routine = StartCoroutine(SwingRoutine());

            IEnumerator SwingRoutine()
            {
                if (JointAxis == null)
                    JointAxis = GetComponent<HingeJoint>();

                JointMotor motor = JointAxis.motor;
                motor.force = motorForce;
                JointAxis.useMotor = true;

                float startAngle = GetLocalXAngle();
                float elapsed = 0f;

                while (elapsed < moveTime)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / moveTime);
                    float smoothT = Mathf.SmoothStep(0f, 1f, t);
                    float currentAngle = Mathf.Lerp(startAngle, targetAngle, smoothT);
                    float diff = currentAngle - GetLocalXAngle();

                    motor.targetVelocity = diff / Time.deltaTime;
                    JointAxis.motor = motor;

                    yield return null;
                }

                motor.targetVelocity = 0;
                JointAxis.motor = motor;
                JointAxis.useMotor = false;

                yield return new WaitForSeconds(holdTime);
                JointAxis.useMotor = false;
                yield return null;
            }
        }

        private void Push(float power)
        {
            if (Rigidbody == null || JointAxis == null)
                return;

            // Drehimpuls um die Achse des Hinge (lokale X-Achse)
            Vector3 torque = JointAxis.transform.right * power;
            Rigidbody.AddTorque(torque, ForceMode.Impulse);
        }

        private float GetLocalXAngle()
        {
            float angle = JointAxis.transform.localEulerAngles.x;
            if (angle > 180f)
                angle -= 360f;
            return angle;
        }
    }
}

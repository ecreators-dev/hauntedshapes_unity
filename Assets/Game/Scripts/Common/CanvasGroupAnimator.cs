using UnityEngine;
using UnityEngine.Playables;

namespace Game.Common.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupAnimator : MonoBehaviour
    {
        [SerializeField] private AnimationCurve alphaOverTime;

        [Min(0.01f)]
        [SerializeField] private float durationSeconds = 3;

        [SerializeField] private AnimationEndModeEnum endAction = AnimationEndModeEnum.Hold;

        private float runtime;
        private AnimationNextStateEnum nextState;
        private CanvasGroup group;
        private bool reversed;

        private enum AnimationEndModeEnum
        {
            Stop = 0, // back to 0
            Repeat = 1, // back to 0 but play
            Ping = 2, // to 0 reversed
            Hold = 3 // stop at end
        }

        private enum AnimationNextStateEnum
        {
            Stop = 0,
            Play = 1,
            Pause = 2,
            Reverse = 3
        }

        private void Awake()
        {
            this.group = GetComponent<CanvasGroup>();
        }

        public void Pause()
        {
            nextState = AnimationNextStateEnum.Pause;
        }

        public void Reverse()
        {
            nextState = AnimationNextStateEnum.Reverse;
            reversed = !reversed;
        }

        public void Stop()
        {
            nextState = AnimationNextStateEnum.Stop;
        }

        public void Play()
        {
            nextState = AnimationNextStateEnum.Play;
        }

        private void Start()
        {
            Stop();
        }

        private void Update()
        {
            BeforeRuntime();

            Animate();

            if (nextState == AnimationNextStateEnum.Stop ||
                nextState == AnimationNextStateEnum.Pause)
                return;

            ChangeRuntime();

            AfterRuntime();

        }

        private void BeforeRuntime()
        {
            if (nextState == AnimationNextStateEnum.Stop)
                runtime = 0;
        }

        private void AfterRuntime()
        {
            bool isEnd = reversed ? (runtime <= 0) : (runtime >= durationSeconds);
            if (isEnd)
            {
                switch (endAction)
                {
                    case AnimationEndModeEnum.Stop:
                        if (reversed)
                            runtime = durationSeconds;
                        else
                            runtime = 0;
                        Stop();
                        break;
                    case AnimationEndModeEnum.Repeat:
                        if (reversed)
                            runtime = durationSeconds;
                        else
                            runtime = 0;
                        break;
                    case AnimationEndModeEnum.Ping:
                        runtime = reversed ? durationSeconds : 0;
                        Reverse();
                        break;
                    case AnimationEndModeEnum.Hold:
                        runtime = reversed ? 0 : durationSeconds;
                        break;
                    default:
                        break;
                }
            }
        }

        private void ChangeRuntime()
        {
            if (nextState == AnimationNextStateEnum.Stop || nextState == AnimationNextStateEnum.Pause)
                return;

            int reversedFactor = 1;
            if (nextState == AnimationNextStateEnum.Reverse && reversed)
            {
                reversedFactor = -1;
            }

            runtime += Time.deltaTime * reversedFactor;
        }

        private void Animate()
        {
            float alpha = alphaOverTime.Evaluate(runtime / durationSeconds) / 1000f;
            group.alpha = alpha;
        }
    }
}

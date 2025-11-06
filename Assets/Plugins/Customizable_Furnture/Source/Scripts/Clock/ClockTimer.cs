using UnityEngine;

public class ClockTimer : MonoBehaviour
{
    [SerializeField] GameObject display;

    public int initialHoursValue;
    public int initialMinutesValue;
    public int initialSecondsValue;
    public float clockSpeed = 1.0f;
    public bool pointFlicker = true;
    public bool reverse = false;

    private DigitalClock digitalClock;
    private AnalogicClock analogicClock;

    public void SetDisplay(bool active)
    {
        this.display.SetActive(active);
    }

    public void SetTime(int hour, int minute, int second)
    {
        this.initialHoursValue = hour;
        this.initialMinutesValue = minute;
        this.initialSecondsValue = second;
        UpdateTime();
    }

    // Use this for initialization
    void OnEnable()
    {
        digitalClock = GetComponentInChildren<DigitalClock>();
        if (digitalClock != null)
        {
            digitalClock.clockSpeed = clockSpeed;
            digitalClock.SetReverseClock(reverse);

            if (!pointFlicker)
            {
                digitalClock.pointFlicker = false;
            }

        }

        analogicClock = GetComponentInChildren<AnalogicClock>();
        if (analogicClock != null)
        {
            analogicClock.clockSpeed = reverse ? -clockSpeed : clockSpeed;
        }

        UpdateTime();
    }

    private void UpdateTime()
    {
        digitalClock?.hoursDCV?.ChangeToTargetTime(initialHoursValue);
        digitalClock?.minutesDCV?.ChangeToTargetTime(initialMinutesValue);
        digitalClock?.secondsDCV?.ChangeToTargetTime(initialSecondsValue);

        analogicClock?.SetTime(initialHoursValue, initialMinutesValue, initialSecondsValue);
    }

    private void Update()
    {
        if (digitalClock == null)
        {
            return;
        }

        digitalClock.clockSpeed = clockSpeed;
    }
}

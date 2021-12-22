using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]
    Image timerFillImg;

    [SerializeField]
    Gradient timerGradient;

    //Timer alarm
    Alarm timerAlarm = new Alarm(1);

    //Timer values;
    [SerializeField]
    float currentTime = 0f;
    float currentDuration = 20f;
    float durationDelta = 1f;
    float durationCap = 1f;

    EventManager.Event timedOutEvent;

    bool timerReset = false;

    public void StartTimer()
    {
        timedOutEvent = EventManager.AddEvent(999, "TimedOut", () =>
        {
            //Another lane will be spawned
            PlayingField.SpawnNewLane();
            
            //Recalculate Timer Values
            RecalculateTimerValues();

            //Update timer duration
            timerAlarm.UpdateDuration(0, currentDuration);
        });

        StartCoroutine(TimerCycle());

        timerAlarm.SetFor(currentDuration, 0, false, () =>
        {
            timedOutEvent.Trigger();
        });
    }

    /// <summary>
    /// Recalculate timer values that effect the speed of gameplay.
    /// </summary>
    void RecalculateTimerValues()
    {
        if (currentDuration > durationCap)
        {
            currentDuration -= durationDelta;
            durationDelta -= PlayingField.CurrentLevel / 10f;
        }
    }

    IEnumerator TimerCycle()
    {
        while (true)
        {
            currentTime = timerAlarm.registeredTimers[0].CurrentTime;
            timerFillImg.fillAmount = currentTime / currentDuration;
            timerFillImg.color = timerGradient.Evaluate(timerFillImg.fillAmount);

            //Give this loop 0.001 secs to refresh image
            yield return new WaitForSeconds(1f / 1000f);
        }
    }
}

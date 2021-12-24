using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField]
    Image timerFillImg;

    [SerializeField]
    TextMeshProUGUI timeTMP;

    [SerializeField]
    Gradient timerGradient, timeGradient;

    //Timer alarm
    Alarm timerAlarm = new Alarm(1);

    //Timer values;
    [SerializeField]
    float currentTime = 0f;

    [SerializeField]
    float currentDuration = 20f;

    DifficultyConfig setConfig = GameManager.SelectedConfig;

    EventManager.Event timedOutEvent;

    public void StartTimer()
    {
        GameManager.PostSetConfig(setConfig);

        timedOutEvent = EventManager.AddEvent(999, "TimedOut", () =>
        {
            //Another lane will be spawned
            PlayingField.SpawnNewLane();
            
            //Recalculate Timer Values
            RecalculateTimerValues();

            //Update timer duration
            timerAlarm.UpdateDuration(0, currentDuration);
        });

        currentDuration = setConfig.InitDuration;

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
        if (currentDuration > setConfig.DurationCap)
        {
            currentDuration -= setConfig.DurationDelta;
            setConfig.DurationDelta += (PlayingField.CurrentLevel / setConfig.LevelDividend);
        } else
        {
            //Even though we are at the cap, this is called the "Bulging Period"
            //where we slowly go beyond the initial cap value that we have.
            GameManager.MarkBulgingPeriod();
            currentDuration = setConfig.DurationCap;
            currentDuration -= PlayingField.CurrentLevel / 1000f;
        }
    }

    public void Stop()
    {
        timerAlarm.Discard();
        StopAllCoroutines();
    }

    IEnumerator TimerCycle()
    {
        while (true)
        {
            currentTime = timerAlarm.registeredTimers[0].CurrentTime;

            timerFillImg.fillAmount = currentTime / currentDuration;
            timerFillImg.color = timerGradient.Evaluate(timerFillImg.fillAmount);

            timeTMP.text = Mathf.FloorToInt(currentDuration - currentTime).ToString();
            timeTMP.color = timerGradient.Evaluate(timerFillImg.fillAmount);

            //Give this loop 0.001 secs to refresh image
            yield return new WaitForSeconds(1f / 1000f);
        }
    }
}

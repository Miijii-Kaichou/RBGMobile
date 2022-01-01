using TMPro;
using UnityEngine;

//FrameRate Options
public enum FrameRate
{
    FPS30 = 30,
    FPS60 = 60,
    FPS120 = 120,
    UNLIMITED = -1
}

public class FPSCounter : Singleton<FPSCounter>
{


    private static int fpsAccumulator = 0;
    private static float fpsNextPeriod = 0f;
    private static int currentFPS;

    //TextMeshPro
    [SerializeField]
    private TextMeshProUGUI fpsText;

    //Constants
    const float fpsMeasurePeriod = 0.25f;
    const string display = "[0] FPS";

    public delegate void FPSUpdateCallbackMethod();
    public static FPSUpdateCallbackMethod FpsUpdateEvent;

    // Start is called before the first frame update
    void Start()
    {
        fpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
    }

    // Update is called once per frame
    void Update()
    {
        //measure average frames per second
        fpsAccumulator++;

        if ( Time.realtimeSinceStartup > fpsNextPeriod)
        {
            currentFPS = (int)(fpsAccumulator / fpsMeasurePeriod);
            fpsAccumulator = 0;
            fpsNextPeriod += fpsMeasurePeriod;

            if (fpsText != null)
                fpsText.text = "[" + currentFPS.ToString() + "] FPS";

            if(FpsUpdateEvent != null) FpsUpdateEvent.Invoke();
        }
    }

    public static int GetCurrectFPS() => currentFPS;
}

using UnityEngine;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    FrameRate targetFrameRate;

    [SerializeField]
    bool vSyncOn;

    [SerializeField, Header("Debug Tools")]
    TextMeshProUGUI chainLengthTMP;

    [SerializeField]
    TextMeshProUGUI targetFPSTMP;

    [SerializeField]
    TextMeshProUGUI refreshRateTMP;

    [SerializeField]
    TextMeshProUGUI FPSTMP;

    [SerializeField]
    TextMeshProUGUI activeBlocksTMP;

    [SerializeField]
    float gravityValue = 0.980665f;

    public static float GravityValue => Instance.gravityValue;

    string chainLengthFormat = "Chain Length: {0}";
    string targetFPSFormat = "Target FPS: {0}";
    string refreshRateFormat = "Refresh Rate: {0}";
    string currentFPSFormat = "FPS: {0}";
    string activeBlocksFormat = "Active Blocks (#/300): {0}";

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = (int)targetFrameRate;
        QualitySettings.vSyncCount = vSyncOn ? 1 : 0;
        targetFPSTMP.text = string.Format(targetFPSFormat, Application.targetFrameRate);
        refreshRateTMP.text = string.Format(refreshRateFormat, Screen.currentResolution.refreshRate);
        FPSCounter.FpsUpdateEvent = PostFPS;

        
    }

    public static void PostChainLength(int value)
    {
        Instance.chainLengthTMP.text = string.Format(Instance.chainLengthFormat, value);
    }

    public static void PostFPS()
    {
        Instance.targetFPSTMP.text = string.Format(Instance.targetFPSFormat, Application.targetFrameRate);
        Instance.FPSTMP.text = string.Format(Instance.currentFPSFormat, FPSCounter.GetCurrectFPS());
    }

    public static void PostActiveBlocks(int value)
    {
        Instance.activeBlocksTMP.text = string.Format(Instance.activeBlocksFormat, value);
    }
}
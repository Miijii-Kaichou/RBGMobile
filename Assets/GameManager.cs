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
    TextMeshProUGUI diffConfigTMP;

    [SerializeField]
    float gravityValue = 0.980665f;

    public static float GravityValue => Instance.gravityValue;

    string chainLengthFormat = "Chain Length: {0}";
    string targetFPSFormat = "Target FPS: {0}";
    string refreshRateFormat = "Refresh Rate: {0}";
    string currentFPSFormat = "FPS: {0}";
    string diffConfigFormat = "Diff Config: {0}";
    string activeBlocksFormat = "Active Blocks (#/300): {0}";

    //Difficulty Configurations
    public static DifficultyConfig[] DiffConfigs { get; private set; } =
    {
       new DifficultyConfig
       (
            "DIFF_CONFIG_NORMAL",
            totalLanes: 3,
            initDuration:  30f,
            durationDelta: 1f,
            durationCap:   5f,
            levelDividend: 32f
       ),

       new DifficultyConfig
       (
            "DIFF_CONFIG_HARD",
            totalLanes: 6,
            initDuration:  30f,
            durationDelta: 2f,
            durationCap:   3f,
            levelDividend: 16f
       ),

       new DifficultyConfig
       (
            "DIFF_CONFIG_EXPERT",
            totalLanes: 9,
            initDuration:  30f,
            durationDelta: 3f,
            durationCap:   1f,
            levelDividend: 8f
       ),
    };

    public static DifficultyConfig SelectedConfig => DiffConfigs[0];

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

    public static void PostSetConfig(DifficultyConfig config)
    {
        Instance.diffConfigTMP.text = string.Format(Instance.diffConfigFormat, config.Tag);
    }

}
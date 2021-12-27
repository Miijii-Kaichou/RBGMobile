using UnityEngine;
using TMPro;
using System;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    FrameRate targetFrameRate;

    [SerializeField]
    bool vSyncOn;

    [SerializeField, Header("Debug Tools")]
    GameObject debugObj;

    [SerializeField]
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
    TextMeshProUGUI bulgingTMP;

    [SerializeField]
    float gravityValue = 0.980665f;

    public static float GravityValue => Instance.gravityValue;

    string chainLengthFormat = "Chain Length: {0}";
    string targetFPSFormat = "Target FPS: {0}";
    string refreshRateFormat = "Refresh Rate: {0}";
    string currentFPSFormat = "FPS: {0}";
    string diffConfigFormat = "Diff Config: {0}";
    string activeBlocksFormat = "Active Blocks (#/300): {0}";

    const int ResWidth = 1080;
    const int ResHeight = 1920;

    //Difficulty Configurations
    public static DifficultyConfig[] DiffConfigs { get; private set; } =
    {
       new DifficultyConfig
       (
            "DIFF_CONFIG_EASY",
            totalLanes: 3,
            initDuration:  15f,
            durationDelta: 0.05f,
            durationCap:   4f,
            levelDividend: 256f
       ),

       new DifficultyConfig
       (
            "DIFF_CONFIG_NORMAL",
            totalLanes: 6,
            initDuration:  15f,
            durationDelta: 0.125f,
            durationCap:   3f,
            levelDividend: 256f
       ),

       new DifficultyConfig
       (
            "DIFF_CONFIG_HARD",
            totalLanes: 12,
            initDuration:  10f,
            durationDelta: 0.25f,
            durationCap:   2f,
            levelDividend: 128f
       ),

       new DifficultyConfig
       (
            "DIFF_CONFIG_EXPERT",
            totalLanes: 18,
            initDuration:  5f,
            durationDelta: 0.5f,
            durationCap:   1f,
            levelDividend: 64f
       ),
    };

    internal static void MarkBulgingPeriod()
    {
        Instance.bulgingTMP.text = "Status: Bulging";
    }

    public static DifficultyConfig SelectedConfig => DiffConfigs[1];

    public static bool EnableDebug { get; internal set; }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = (int)targetFrameRate;
        QualitySettings.vSyncCount = vSyncOn ? 1 : 0;
        FPSCounter.FpsUpdateEvent = PostFPS;

        if (!EnableDebug)
        {
            debugObj.SetActive(EnableDebug);
            return;
        }
        
        targetFPSTMP.text = string.Format(targetFPSFormat, Application.targetFrameRate);
        refreshRateTMP.text = string.Format(refreshRateFormat, Screen.currentResolution.refreshRate);
    }

    public static void PostChainLength(int value)
    {
        if (!EnableDebug) return;
        Instance.chainLengthTMP.text = string.Format(Instance.chainLengthFormat, value);
    }

    public static void PostFPS()
    {
        if (!EnableDebug) return;
        Instance.targetFPSTMP.text = string.Format(Instance.targetFPSFormat, Application.targetFrameRate);
        Instance.FPSTMP.text = string.Format(Instance.currentFPSFormat, FPSCounter.GetCurrectFPS());
    }

    public static void PostActiveBlocks(int value)
    {
        if (!EnableDebug) return;
        Instance.activeBlocksTMP.text = string.Format(Instance.activeBlocksFormat, value);
    }

    public static void PostSetConfig(DifficultyConfig config)
    {
        if (!EnableDebug) return;
        Instance.diffConfigTMP.text = string.Format(Instance.diffConfigFormat, config.Tag);
    }

}
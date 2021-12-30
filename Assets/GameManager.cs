using UnityEngine;
using TMPro;
using System;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    FrameRate targetFrameRate;

    [SerializeField]
    bool vSyncOn;

    [SerializeField]
    public bool _enableDebug = false;
    public static bool EnableDebug => Instance._enableDebug;

    internal static void SetDiffConfig(int difficulty)
    {
        SelectedConfig = DiffConfigs[difficulty];
    }

    [SerializeField]
    float gravityValue = 0.980665f;

    public static float GravityValue => Instance.gravityValue;


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

  
    public static DifficultyConfig SelectedConfig { get; private set; }


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = (int)targetFrameRate;
        QualitySettings.vSyncCount = vSyncOn ? 1 : 0;
        
    }
}
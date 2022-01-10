using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameSessionDebugger : MonoBehaviour
{
    private static GameSessionDebugger Instance;

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

    

    string chainLengthFormat = "Chain Length: {0}";
    string targetFPSFormat = "Target FPS: {0}";
    string refreshRateFormat = "Refresh Rate: {0}";
    string currentFPSFormat = "FPS: {0}";
    string diffConfigFormat = "Diff Config: {0}";
    string activeBlocksFormat = "Active Blocks (#/300): {0}";

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (!GameManager.EnableDebug)
        {
            debugObj.SetActive(GameManager.EnableDebug);
            return;
        }

        FPSCounter.FpsUpdateEvent = PostFPS;
        targetFPSTMP.text = string.Format(targetFPSFormat, Application.targetFrameRate);
        refreshRateTMP.text = string.Format(refreshRateFormat, Screen.currentResolution.refreshRate);
    }

    internal static void MarkBulgingPeriod()
    {
        Instance.bulgingTMP.text = "Status: Bulging";
    }

    public static void PostChainLength(int value)
    {
        if (!GameManager.EnableDebug) return;
        Instance.chainLengthTMP.text = string.Format(Instance.chainLengthFormat, value);
    }

    public static void PostFPS()
    {
        if (!GameManager.EnableDebug) return;
        Instance.targetFPSTMP.text = string.Format(Instance.targetFPSFormat, Application.targetFrameRate);
        Instance.FPSTMP.text = string.Format(Instance.currentFPSFormat, FPSCounter.GetCurrectFPS());
    }

    public static void PostActiveBlocks(int value)
    {
        if (!GameManager.EnableDebug) return;
        Instance.activeBlocksTMP.text = string.Format(Instance.activeBlocksFormat, value);
    }

    public static void PostSetConfig(DifficultyConfig config)
    {
        if (!GameManager.EnableDebug) return;
        Instance.diffConfigTMP.text = string.Format(Instance.diffConfigFormat, config.Tag);
    }
}

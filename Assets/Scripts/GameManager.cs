using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using PlayFab.CloudScriptModels;
using TMPro;
using System;
using System.Collections.Generic;
using static Extensions.Convenience;

public class GameManager : Singleton<GameManager>
{

    public PlayerModel playerModel;

    [SerializeField]
    FrameRate targetFrameRate;

    [SerializeField]
    bool vSyncOn;

    [SerializeField]
    public bool _enableDebug = false;
    public static bool EnableDebug => Instance._enableDebug;

    public static Dictionary<string, int> VirtualCurrency = new Dictionary<string, int>();

    

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
            levelDividend: 256f,
            influence: 0.25f
       ),

       new DifficultyConfig
       (
            "DIFF_CONFIG_NORMAL",
            totalLanes: 6,
            initDuration:  15f,
            durationDelta: 0.125f,
            durationCap:   3f,
            levelDividend: 256f,
            influence: 0.5f
       ),

       new DifficultyConfig
       (
            "DIFF_CONFIG_HARD",
            totalLanes: 12,
            initDuration:  10f,
            durationDelta: 0.25f,
            durationCap:   2f,
            levelDividend: 128f,
            influence: 1
       ),

       new DifficultyConfig
       (
            "DIFF_CONFIG_EXPERT",
            totalLanes: 18,
            initDuration:  5f,
            durationDelta: 0.5f,
            durationCap:   1f,
            levelDividend: 64f,
            influence: 2
       ),
    };


    public static DifficultyConfig SelectedConfig { get; private set; }


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = (int)targetFrameRate;
        QualitySettings.vSyncCount = vSyncOn ? 1 : 0;

    }

    public static PlayerModel PlayerModel => Instance.playerModel;

    #region Client-API Calls

    public static void PostError(PlayFabError error)
    {
        Debug.Log($"Failed! [REASON: {error.ErrorMessage}] [EXIT CODE: {error.HttpCode}]");
    }

    public static void InitializePlayerStatistics(PlayFabResultCommon result)
    {
        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new System.Collections.Generic.List<StatisticUpdate>()
            {
                new StatisticUpdate() {StatisticName = "SoloModePlayCount", Value = 0 },
                new StatisticUpdate(){StatisticName = "SurvivalModePlayCount", Value = 0 },
                new StatisticUpdate(){StatisticName = "WipeOutPlayCount", Value = 0 },
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, GetInitialzeVirtualCurrency, PostError);
    }

    public static void GetInitialzeVirtualCurrency(PlayFabResultCommon result)
    {

        PlayFabClientAPI.GetUserInventory(
            new GetUserInventoryRequest { },
            ok => { GameManager.VirtualCurrency = ok.VirtualCurrency; GotoMainSelection(ok); },
            err => PostError(err)
        );
    }

    static void GotoMainSelection(PlayFabResultCommon success)
    {
        Debug.Log("Proceeding to Main Menu");
        PlayerModel.BlooxCurrency = GameManager.VirtualCurrency["BX"];
        GameSceneManager.Deploy();
    }


    public static void PushToRemotePlayerModel(Action<UpdateUserDataResult> success, Action<PlayFabError> fail)
    {
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>
            {
                { "FirstTimeUser", PlayerModel.FirstTimeUser.ToString() },
                { "UserName", PlayerModel.UserName },
                { "UniqueIdentifier", PlayerModel.UniqueIdentifier },
                { "PlayerAvatar", PlayerModel.PlayerAvatar.ToString() },
                { "RGBTheme", PlayerModel.PlayerTheme.ToString() },
                { "ExperiencePoints", PlayerModel._PlayerExperience.ToString() },
                { "Recoverable", PlayerModel.HasRecoverableAccount.ToString() },
            },
        };


        PlayFabClientAPI.UpdateUserData(request, ok =>
        {
            Debug.Log("PlayerModel Successfully Pushed");
            success.Invoke(ok);
        }, err =>
        {
            Debug.Log($"Failed to Push PlayerModel: [REASON:{err.ErrorMessage}] [EXIT CODE:{err.HttpCode}]");
            fail.Invoke(err);
        });
    }

    public static void PostPlayerPlayCountStatistics(Action<PlayFabResultCommon> success = null, Action<PlayFabError> fail = null)
    {
        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate() {StatisticName = "SoloModePlayCount", Value = PlayerModel.TimesPlayedSolo },
                new StatisticUpdate(){StatisticName = "SurvivalModePlayCount", Value = PlayerModel.TimesPlayedSurvival },
                new StatisticUpdate(){StatisticName = "WipeOutPlayCount", Value = PlayerModel.TimesPlayedWipeOut },
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, success, fail);
    }

    #region Not Implemented
    //public static void PostPlayerBestStatistics(PlayFabResultCommon result, int soloBest, int survivalBest, int wipeOutBest, Action<PlayFabResultCommon> success = null, Action<PlayFabError> fail = null)
    //{
    //    var request = new UpdatePlayerStatisticsRequest()
    //    {
    //        Statistics = new List<StatisticUpdate>()
    //        {
    //            new StatisticUpdate() {StatisticName = "SoloBest", Value = soloBest },
    //            new StatisticUpdate(){StatisticName = "SurvivalBest", Value = survivalBest },
    //            new StatisticUpdate(){StatisticName = "WipeOutBest", Value = wipeOutBest},
    //        }
    //    };

    //    PlayFabClientAPI.UpdatePlayerStatistics(request, success, fail);
    //}

    #endregion

    public static void RequestPlayerModel(string pfID, Action<GetUserDataResult> success, Action<PlayFabError> fail)
    {
        //Set up GET REQUEST
        var request = new GetUserDataRequest()
        {
            PlayFabId = pfID,
            Keys = null
        };

        //Call ClientAPI to fetch data
        PlayFabClientAPI.GetUserData
        (
            request,
            ok =>
            {
                PlayerModel.FirstTimeUser = ok.Data["FirstTimeUser"].Value.ToBool();
                PlayerModel.UserName = ok.Data["UserName"].Value;
                PlayerModel.UniqueIdentifier = ok.Data["UniqueIdentifier"].Value;
                PlayerModel.PlayerAvatar = ok.Data["PlayerAvatar"].Value.ToInt();
                PlayerModel.PlayerTheme = ok.Data["RGBTheme"].Value.ToInt();
                PlayerModel.PlayerExperience = ok.Data["ExperiencePoints"].Value.ToFloat();
                PlayerLevelManager.LevelExperiencePoints = ok.Data["ExperiencePoints"].Value.ToFloat();
                Debug.Log(ok.Data["ExperiencePoints"].Value.ToFloat());
                PlayerModel.HasRecoverableAccount = ok.Data["Recoverable"].Value.ToBool();
                success.Invoke(ok);
            },
            err =>
            {
                fail.Invoke(err);
            }
        );
    }
    #endregion
}

#if UNITY_EDITOR
#endif
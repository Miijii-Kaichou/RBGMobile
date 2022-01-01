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
    [SerializeField]
    PlayerModel _playerModel;

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

    public static PlayerModel PlayerModel => Instance._playerModel;

    #region Cloud-Scripts Calls
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
        if (PlayerModel.FirstTimeUser)
        {
            //Automatic Success Invocation. 
            //The class responsible for call will handle
            //the rest.
            success.Invoke(null);
            return;
        }

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
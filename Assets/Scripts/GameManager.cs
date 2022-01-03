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
    #region Serialized Fields

    public PlayerModel playerModel;

    [SerializeField]
    FrameRate targetFrameRate;

    [SerializeField]
    bool vSyncOn;

    [SerializeField]
    public bool _enableDebug = false;

    [SerializeField]
    float gravityValue = 0.980665f;
    #endregion

    #region Static Properties/Fields
    public static bool EnableDebug => Instance._enableDebug;

    public static Dictionary<string, int> VirtualCurrency = new Dictionary<string, int>();

    public static float GravityValue => Instance.gravityValue;

    static Dictionary<string, int> RequestedStatistics = new Dictionary<string, int>();
    static List<string> statisticNames = new List<string>()
            {
                "SoloModePlayCount",
                "SurvivalModePlayCount",
                "WipeOutPlayCount",
                "SoloEasyBestScore",
                "SoloNormalBestScore",
                "SoloHardBestScore",
                "SoloExpertBestScore"
            };

    //Difficulty Configurations
    public static DifficultyConfig[] DiffConfigs { get; private set; } =
    {
       new DifficultyConfig
       (
            tag: "DIFF_CONFIG_EASY",
            id: 0,
            totalLanes: 3,
            initDuration:  15f,
            durationDelta: 0.05f,
            durationCap:   4f,
            levelDividend: 256f,
            influence: 0.25f
       ),

       new DifficultyConfig
       (
            tag: "DIFF_CONFIG_NORMAL",
            id: 1,
            totalLanes: 6,
            initDuration:  15f,
            durationDelta: 0.125f,
            durationCap:   3f,
            levelDividend: 256f,
            influence: 0.5f
       ),

       new DifficultyConfig
       (
            tag: "DIFF_CONFIG_HARD",
            id: 2,
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
            id: 3,
            totalLanes: 18,
            initDuration:  5f,
            durationDelta: 0.5f,
            durationCap:   1f,
            levelDividend: 64f,
            influence: 2
       ),
    };
    public static DifficultyConfig SelectedConfig { get; private set; }

    #endregion

    #region Constants
    const int ResWidth = 1440;
    const int ResHeight = 2960;
    #endregion

    internal static void SetDiffConfig(int difficulty)
    {
        SelectedConfig = DiffConfigs[difficulty];
    }


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = (int)targetFrameRate;
        QualitySettings.vSyncCount = vSyncOn ? 1 : 0;
        Screen.SetResolution(Screen.width, Screen.height, true, (int)targetFrameRate);
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
            Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate() {StatisticName = "SoloModePlayCount", Value = 0 },
                new StatisticUpdate(){StatisticName = "SurvivalModePlayCount", Value = 0 },
                new StatisticUpdate(){StatisticName = "WipeOutPlayCount", Value = 0 },
                new StatisticUpdate(){StatisticName = "SoloEasyBestScore", Value = 0 },
                new StatisticUpdate(){StatisticName = "SoloNormalBestScore", Value = 0 },
                new StatisticUpdate(){StatisticName = "SoloHardBestScore", Value = 0 },
                new StatisticUpdate(){StatisticName = "SoloExpertBestScore", Value = 0 },

            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, GetVirtualCurrency, PostError);
    }

    public static void GetVirtualCurrency(PlayFabResultCommon result)
    {

        PlayFabClientAPI.GetUserInventory(
            new GetUserInventoryRequest { },
            ok => { GotoMainSelection(ok); },
            err => PostError(err)
        );
    }

    public static void GetVirtualCurrency(Action<GetUserInventoryResult> success, Action<PlayFabError> fail)
    {

        PlayFabClientAPI.GetUserInventory(
            new GetUserInventoryRequest { },
            success,
            fail
        );
    }

    static void GotoMainSelection(GetUserInventoryResult success)
    {
        VirtualCurrency = success.VirtualCurrency;
        PlayerModel.BlooxCurrency = VirtualCurrency["BX"];
        PlayerModel.PrizmCurrency = VirtualCurrency["PZ"];
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

    public static void PostPlayerBestStatistics(int soloBest, int difficulty, Action<PlayFabResultCommon> success = null, Action<PlayFabError> fail = null)
    {
        string[] soloPlayModes =
        {
            "Easy",
            "Normal",
            "Hard",
            "Expert",
        };
        PlayerModel.SoloBestScores[difficulty] = soloBest;
        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate() {StatisticName = $"Solo{soloPlayModes[difficulty]}BestScore", Value = PlayerModel.SoloBestScores[difficulty]},
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, success, fail);

    }

    public static void RequestPlayerBestStatistics(Action<GetPlayerStatisticsResult> success = null, Action<PlayFabError> fail = null)
    {

        var request = new GetPlayerStatisticsRequest()
        {
            StatisticNames = new List<string>()
            {
                "SoloModePlayCount",
                "SurvivalModePlayCount",
                "WipeOutPlayCount",
                "SoloEasyBestScore",
                "SoloNormalBestScore",
                "SoloHardBestScore",
                "SoloExpertBestScore"
            }
        };

        PlayFabClientAPI.GetPlayerStatistics(request,
            (ok) =>
            {
                PassStats(ok, success);
            }, (err) =>
            {
                fail.Invoke(err);
            });
    }

    static void PassStats(GetPlayerStatisticsResult ok, Action<GetPlayerStatisticsResult> success)
    {

        foreach (StatisticValue stat in ok.Statistics)
        {
            RequestedStatistics.Add(stat.StatisticName, stat.Value);
        }

        int i = 0;
        //Update Player Model
        PlayerModel.TimesPlayedSolo = RequestedStatistics[statisticNames[i++]];
        PlayerModel.TimesPlayedSurvival = RequestedStatistics[statisticNames[i++]];
        PlayerModel.TimesPlayedWipeOut = RequestedStatistics[statisticNames[i++]];
        for (int j = 0; j < PlayerModel.SoloBestScores.Length; j++)
        {
            PlayerModel.SoloBestScores[j] = RequestedStatistics[statisticNames[i++]];
        }

        //Invoke Successful Stat Update
        success.Invoke(ok);
    }

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
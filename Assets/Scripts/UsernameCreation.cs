using System;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using UnityEngine;
using TMPro;
using System.Text;

using Random = UnityEngine.Random;

public class UsernameCreation : MonoBehaviour
{
    [SerializeField]
    TMP_InputField userNameField;

    public void GenerateTempPassword(out string result)
    {
        result = "";
        StringBuilder tempPassword = new StringBuilder(result);
        Random.InitState((int)System.DateTime.Now.Ticks);
        for(int i = 0; i < 32; i++)
        {
            tempPassword.Append((char)Random.Range(33, 128));
        }
        result = tempPassword.ToString();
    }

    public void CreateNewAccount()
    {
        GameSceneManager.PrepareToLoad(2);
        Proceed();
    }

    void Proceed()
    {
        Debug.Log("Proceeding");

        //Update PlayerModel
        GameManager.PlayerModel.FirstTimeUser = true;
        GameManager.PlayerModel.BlooxCurrency = 1000;
        GameManager.PlayerModel.UniqueIdentifier = GetUniqueIdentifier();
        GameManager.PlayerModel.UserName = userNameField.text;
        GameManager.PlayerModel.PlayerAvatar = 0;
        GameManager.PlayerModel.PlayerTheme = 0;
        GameManager.PlayerModel.PlayerRank = 1;
        GameManager.PlayerModel.TimesPlayedSolo = 0;
        GameManager.PlayerModel.TimesPlayedSurvival = 0;
        GameManager.PlayerModel.TimesPlayedWipeOut = 0;
        GameManager.PlayerModel.HasRecoverableAccount = false;

        //Push Modified PlayerModel to Playfab Title
        GameManager.PushToRemotePlayerModel(InitializePlayerStatistics, PostError);
    }

    void PostError(PlayFabError error)
    {
        Debug.Log($"Failed! [REASON: {error.ErrorMessage}] [EXIT CODE: {error.HttpCode}]");
    }

    public void RecoverAccountInfo()
    {

    }

    void InitializePlayerStatistics(PlayFabResultCommon result)
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

    void GetInitialzeVirtualCurrency(PlayFabResultCommon result)
    {
        
        PlayFabClientAPI.GetUserInventory(
            new GetUserInventoryRequest { },
            ok => { GameManager.VirtualCurrency = ok.VirtualCurrency; GotoMainSelection(ok); },
            err => PostError(err)
        );
    }
    
    void GotoMainSelection(PlayFabResultCommon success)
    {
        Debug.Log("Proceeding to Main Menu");
        GameManager.PlayerModel.BlooxCurrency = GameManager.VirtualCurrency["Bloox"];
        GameSceneManager.Deploy();
    }

    private string GetUniqueIdentifier()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }
}
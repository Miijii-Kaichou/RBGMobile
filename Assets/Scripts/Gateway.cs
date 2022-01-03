using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using System;

/// <summary>
/// Gateway class will check if the user is a First-Time Player.
/// If they are, they will be sent to the Privacy and Agreements scene, then
/// to the Username scene to create a username.
/// 
/// If they are not a first time player, the player is logged in, and is sent to the Mainmenu
/// </summary>
public class Gateway : MonoBehaviour
{
    private void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
            PlayFabSettings.staticSettings.TitleId = "D35D1";
        }
        //var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        //PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);

        LoadPlayerData();
    }

    public void LoadPlayerData()
    {
#if UNITY_ANDROID
        var androidRequest = new LoginWithAndroidDeviceIDRequest
        {
            AndroidDeviceId = GetUniqueIdentifier(),
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithAndroidDeviceID(androidRequest, OnMobileLogInSuccess, OnMobileLoginFailure);
#endif //ANDROID_DEVICE
        //---------------------------------------------------------------------------------
#if UNITY_IOS

#endif //UNITY_IOS
    }

    private void OnMobileLoginFailure(PlayFabError result)
    {
        
    }

    private void OnMobileLogInSuccess(LoginResult result)
    {
        //Begin the Player Data Retrieval Process
        Debug.Log("Retrieving Player Data...");
        GameManager.RequestPlayerModel(result.PlayFabId, FetchPlayerStatistics, PostError);
    }

    #region Player Data Retrieval Process
    /// <summary>
    /// Return the Player Statistics Data. This happens after the Player Model has been retrieved.
    /// </summary>
    /// <param name="result"></param>
    private void FetchPlayerStatistics(PlayFabResultCommon result)
    {
        GameManager.RequestPlayerBestStatistics(FetchPlayerVirtualCurrencty, PostError);
    }

    /// <summary>
    /// Return the player's Virtual Currency. This happen after the Player Statistics has been retrieved.
    /// </summary>
    /// <param name="result"></param>
    private void FetchPlayerVirtualCurrencty(PlayFabResultCommon result)
    {
        GameManager.GetVirtualCurrency(ValidateFirstTimeUser, PostError);
    }

    /// <summary>
    /// The last stage of the Player Data Retrieval Process. 
    /// This happens after all Player Data (Player Model, Stats, Virtual Currency) has been retrieved
    /// </summary>
    /// <param name="result"></param>
    public void ValidateFirstTimeUser(PlayFabResultCommon result)
    {
        Debug.Log("Player Data Successfully Retrieved!");

        

        //Check if this is the first time player had played this game, not by if it is his first time, but if
        //there is no username.
        if (GameManager.PlayerModel.UserName == string.Empty)
        {
            Debug.Log(GameManager.PlayerModel.FirstTimeUser);
            //Player is first time player
            GameSceneManager.PrepareToLoad(1);

        }
        else
        {
            PlayerLevelManager.LevelExperiencePoints = GameManager.PlayerModel.PlayerExperience;
            GameManager.VirtualCurrency = (result as GetUserInventoryResult).VirtualCurrency;
            GameManager.PlayerModel.BlooxCurrency = GameManager.VirtualCurrency["BX"];
            GameManager.PlayerModel.PrizmCurrency = GameManager.VirtualCurrency["PZ"];

            //Player account exists, and player has username
            GameSceneManager.PrepareToLoad(2);
        }
        GameSceneManager.Deploy();
    } 
    #endregion

    public void PostError(PlayFabError err)
    {
        if(GameManager.PlayerModel.UserName != string.Empty)
        {
            //Can't continue forward. An error had occured
            Application.Quit();
        }

        //This means they weren't in the Playfab database. Proceed to
        //Privacy and Consent Policy Guidelines and Terms and Agreements
        GameSceneManager.PrepareToLoad(1);
        GameSceneManager.Deploy();
    }

    private string GetUniqueIdentifier()
    {
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        return deviceID;
    }
}
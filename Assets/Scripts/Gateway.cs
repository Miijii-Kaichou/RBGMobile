using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
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
        //FetchPlayerModelData
        GameManager.RequestPlayerModel(result.PlayFabId, ValidateFirstTimeUser, PostError);
        
       
    }

    public void ValidateFirstTimeUser(GetUserDataResult result)
    {
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

            //Player account exists, and player has username
            GameSceneManager.PrepareToLoad(2);
        }
        GameSceneManager.Deploy();
    }

    public void PostError(PlayFabError err)
    {
        //This mean it failed to read, so it's going to have to create a new account
        GameSceneManager.PrepareToLoad(1);
        GameSceneManager.Deploy();
    }

    private string GetUniqueIdentifier()
    {
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        return deviceID;
    }
}
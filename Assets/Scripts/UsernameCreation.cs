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
        GameManager.PlayerModel.UniqueIdentifier = GetUniqueIdentifier();
        GameManager.PlayerModel.UserName = userNameField.text;
        GameManager.PlayerModel.PlayerAvatar = 0;
        GameManager.PlayerModel.PlayerTheme = 0;
        GameManager.PlayerModel.TimesPlayedSolo = 0;
        GameManager.PlayerModel.TimesPlayedCrusades = 0;
        GameManager.PlayerModel.TimesPlayedVerses = 0;
        GameManager.PlayerModel.HasRecoverableAccount = false;

        //Push Modified PlayerModel to Playfab Title
        GameManager.PushToRemotePlayerModel(GameManager.InitializePlayerStatistics, GameManager.PostError);
    }

  

    public void RecoverAccountInfo()
    {

    }

    private string GetUniqueIdentifier()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }
}
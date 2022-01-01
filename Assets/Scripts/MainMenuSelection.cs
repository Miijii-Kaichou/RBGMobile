using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class MainMenuSelection : MonoBehaviour
{

    public void OnEnable()
    {
        GameManager.PushToRemotePlayerModel(UpdateStats, PostError);
    }

    void UpdateStats(UpdateUserDataResult result)
    {
        GameManager.PostPlayerPlayCountStatistics(
            (success) => { Debug.Log("PlayerStats Successfully Updated"); },
            PostError);
    }

    void PostError(PlayFabError error)
    {
        Debug.LogError($"Player Stats Update Failed: [REASON:{error.ErrorMessage}] [EXIT CODE: {error.HttpCode}]");
    }

    public void OnSolo()
    {
        GameOverlay.DisableOverlay();
        GameSceneManager.LoadScene(3);
    }

    public void OnSurvival() { }

    public void OnWipeOut() { }
    public void OnLeaderBoard() { }
    public void OnInbox() { }
    public void OnFriendList() { }
    public void OnMenuHome() { }
    public void OnMenuRGBShop() { }
    public void OnMenuCollections() { }
    public void OnMenuGoals() { }
    public void OnGameSettings() { }
    public void OnRewards() { }
    public void OnBanner() { }

}

using UnityEngine;

public class MainMenuSelection : MonoBehaviour
{
    public void OnSolo()
    {
        GameOverlay.DisableOverlay();
        GameSceneManager.LoadScene(2);
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

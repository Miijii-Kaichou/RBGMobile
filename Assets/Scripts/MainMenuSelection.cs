using UnityEngine;

public class MainMenuSelection : MonoBehaviour
{

    public void OnEnable()
    {
        GameManager.SetFPS(FrameRate.FPS30);
        GameOverlay.Reinstate();
    }

    

    public void OnSolo()
    {
        GameOverlay.DisableOverlay();
        GameSceneManager.LoadScene(3);
    }

    public void OnSurvival() { }

    public void OnWipeOut() { }
    public void OnLeaderBoard() { }
    public void OnInbox()
    {
        GameOverlay.DisableOverlay(1);
        GameSceneManager.LoadScene(5);
    }
    public void OnFriendList() { }
    public void OnMenuHome() { }
    public void OnMenuRGBShop() { }
    public void OnMenuCollections() { }
    public void OnMenuGoals() { }
    public void OnGameSettings() { }
    public void OnRewards() { }
    public void OnBanner() { }

}

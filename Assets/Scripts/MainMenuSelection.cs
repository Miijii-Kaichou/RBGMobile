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
        //On on Solo, PlayStyle will always be Survival
        GameManager.SetPlayMode(PlayMode.Solo);
        GameManager.SetPlayStyle(PlayStyle.Survival);

        GameOverlay.DisableOverlay();
        GameSceneManager.LoadScene(3, true);
    }

    public void OnCrusade() 
    {
        //PlayStyle can be specifically configured in PlayModeSelection
        GameManager.SetPlayMode(PlayMode.Crusades);
        GotoPlayModeSelection();
    }

    public void OnVerses()
    {
        //PlayStyle can be specifically configured in PlayModeSelection
        GameManager.SetPlayMode(PlayMode.Verses);
        GotoPlayModeSelection();
    }

    void GotoPlayModeSelection()
    {
        GameOverlay.DisableOverlay();
        GameSceneManager.LoadScene(6, true);
    }
    public void OnLeaderBoard() { }
    public void OnInbox()
    {
        GameOverlay.DisableOverlay(1);
        GameSceneManager.LoadScene(5, true);
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

using UnityEngine;

public enum PlayMode
{
    Solo = 4,
    Survival = 5,
    WipeOut = 6
}

public class GameSessionLauncher : MonoBehaviour
{
    public PlayMode SelectedPlayMode = PlayMode.Solo;

    private void OnEnable()
    {
        GameSceneManager.PrepareToLoad((int)SelectedPlayMode);
    }

    public void LaunchGame(int difficulty)
    {
        GameManager.SetFPS(FrameRate.FPS60);

        GameManager.SetDiffConfig(difficulty);
        switch (SelectedPlayMode)
        {
            case PlayMode.Solo:
                GameManager.PlayerModel.TimesPlayedSolo++;
                break;

            case PlayMode.Survival:
                GameManager.PlayerModel.TimesPlayedSurvival++;
                break;

            case PlayMode.WipeOut:
                GameManager.PlayerModel.TimesPlayedWipeOut++;
                break;

            default:
                break;
        }

        GameSceneManager.LoadScene((int)SelectedPlayMode);
        GameManager.PlayerModel.FirstTimeUser = false;
    }

    public void Back()
    {
        GameSceneManager.LoadPrevious();
        GameOverlay.EnableOverlay();
    }
}

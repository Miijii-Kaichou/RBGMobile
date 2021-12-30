using UnityEngine;

public enum PlayMode
{
    Solo,
    Survival,
    WipeOut
}

public class GameSessionLauncher : MonoBehaviour
{
    public void LaunchGame(int difficulty)
    {
        GameManager.SetDiffConfig(difficulty);
        GameSceneManager.LoadScene(3);
    }

    public void Back()
    {
        GameSceneManager.LoadPrevious();
    }
}

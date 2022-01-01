using UnityEngine;

public class GameOverlay : Singleton<GameOverlay>
{
    [SerializeField]
    GameObject PlayerOverlay, MenuOverlay;

    public static void DisableOverlay()
    {
        Instance.PlayerOverlay.SetActive(false);
        Instance.MenuOverlay.SetActive(false);
    }
    public static void EnableOverlay()
    {
        Instance.PlayerOverlay.SetActive(!false);
        Instance.MenuOverlay.SetActive(!false);
    }
}

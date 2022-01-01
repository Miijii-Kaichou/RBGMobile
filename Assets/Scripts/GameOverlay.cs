using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOverlay : Singleton<GameOverlay>
{
    [SerializeField]
    GameObject PlayerOverlay, MenuOverlay;

    [SerializeField]
    TextMeshProUGUI playerUserNameTMP, playerBlooxValuesTMP, playerLevelTMP;

    [SerializeField]
    Image playerAvatarImage;

    public void OnEnable()
    {
        playerUserNameTMP.text = GameManager.PlayerModel.UserName;
        //playerBlooxValuesTMP.text = GameManager.PlayerModel.BlooxCurrency.ToString();
        playerLevelTMP.text = $"LV {GameManager.PlayerModel.PlayerLevel}";
        playerAvatarImage.sprite = Sprite.Create(AvatarCollectionLog.GetAvatar(GameManager.PlayerModel.PlayerAvatar), new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
    }

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

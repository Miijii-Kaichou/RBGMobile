using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOverlay : Singleton<GameOverlay>
{
    [SerializeField]
    GameObject PlayerOverlay, MenuOverlay;

    [SerializeField]
    TextMeshProUGUI playerUserNameTMP, playerLevelTMP;

    [SerializeField]
    Image playerAvatarImage;

    [SerializeField]
    Image levelFillMeter;

    public static void Reinstate()
    {
        Instance.playerUserNameTMP.text = GameManager.PlayerModel.UserName;
        //playerBlooxValuesTMP.text = GameManager.PlayerModel.BlooxCurrency.ToString();
        Instance.playerLevelTMP.text = $"LV {PlayerLevelManager.CurrentLevel}";
        Instance.levelFillMeter.fillAmount = PlayerLevelManager.Percentage;
        Texture2D avatar = AvatarCollectionLog.GetAvatar(GameManager.PlayerModel.PlayerAvatar);
        Instance.playerAvatarImage.sprite = Sprite.Create(avatar, new Rect(0, 0, avatar.width, avatar.height), new Vector2(0.5f, 0.5f));
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

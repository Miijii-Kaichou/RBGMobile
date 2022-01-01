using UnityEngine;
[CreateAssetMenu(fileName = "New Player Model", menuName = "Player Model")]
public class PlayerModel : ScriptableObject
{
    public bool FirstTimeUser = true;
    public string UserName;
    public string UniqueIdentifier;
    public int BlooxCurrency = 0;
    public int PlayerAvatar;
    public int PlayerTheme;
    public float PlayerExperience;
    public float _PlayerExperience
    {

        get
        {
            PlayerExperience = PlayerLevelManager.LevelExperiencePoints;
            return PlayerExperience;
        }
        private set
        {
            PlayerExperience = value;
        }
    }
    public int PlayerEXUntilNextLevel = 90;
    public int TimesPlayedSolo = 0;
    public int TimesPlayedWipeOut = 0;
    public int TimesPlayedSurvival = 0;
    public int TotalModesPlayed = 0;
    public int _TotalModesPlayed
    {

        get
        {
            TotalModesPlayed = TimesPlayedSolo +
            TimesPlayedSurvival +
            TimesPlayedWipeOut;
            return TotalModesPlayed;
        }
        private set
        {
            TotalModesPlayed = value;
        }
    }


    public void SetTotalModePlayed(int value) => TotalModesPlayed = value;
    public bool HasRecoverableAccount = false;

    public void Reset()
    {
        PlayerLevelManager.Reset();
        FirstTimeUser = true;
        UserName = default;
        UniqueIdentifier = default;
        BlooxCurrency = default;
        PlayerAvatar = default;
        PlayerTheme = default;
        PlayerExperience = default;
        PlayerEXUntilNextLevel = 90;
        TimesPlayedSolo = default;
        TimesPlayedWipeOut = default;
        TimesPlayedSurvival = default;
        TotalModesPlayed = default;
    }
}

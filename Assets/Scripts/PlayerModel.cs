using UnityEngine;

[CreateAssetMenu(fileName = "New Player Model", menuName = "Player Model")]
public class PlayerModel : ScriptableObject
{
    public bool FirstTimeUser = true;
    public string UserName;
    public string UniqueIdentifier;
    public int BlooxCurrency  = 0;
    public int PlayerAvatar;
    public int PlayerTheme;
    public int PlayerLevel = 1;
    public int PlayerEXUntilNextLevel = 90;
    public int TimesPlayedSolo  = 0;
    public int TimesPlayedWipeOut  = 0;
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
    public bool HasRecoverableAccount  = false;

}

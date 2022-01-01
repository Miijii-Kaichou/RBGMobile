using UnityEngine;
using UnityEditor;
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

[CustomEditor(typeof(PlayerModel))]
public class PlayerModeScriptableObjectInspector : Editor
{
    SerializedProperty
        _firstTimeUser,
        _userName,
        _uniqueIdentifier,
        _blooxCurrency,
        _playerAvatar,
        _playerTheme,
        _playerLevel,
        _playerEXUntilNextLevel,
        _timesPlayedSolo,
        _timesPlayedWipeOut,
        _timesPlayedSurvival,
        _totalModesPlayed,
        _hasRecoverableAccount;

    public void OnEnable()
    {
        _firstTimeUser = serializedObject.FindProperty("FirstTimeUser");
        _userName = serializedObject.FindProperty("UserName");
        _uniqueIdentifier = serializedObject.FindProperty("UniqueIdentifier");
        _blooxCurrency = serializedObject.FindProperty("BlooxCurrency");
        _playerAvatar = serializedObject.FindProperty("PlayerAvatar");
        _playerTheme = serializedObject.FindProperty("PlayerTheme");
        _playerLevel = serializedObject.FindProperty("PlayerLevel");
        _playerEXUntilNextLevel = serializedObject.FindProperty("PlayerEXUntilNextLevel");
        _timesPlayedSolo = serializedObject.FindProperty("TimesPlayedSolo");
        _timesPlayedWipeOut = serializedObject.FindProperty("TimesPlayedWipeOut");
        _timesPlayedSurvival = serializedObject.FindProperty("TimesPlayedSurvival");
        _totalModesPlayed = serializedObject.FindProperty("TotalModesPlayed");
        _hasRecoverableAccount = serializedObject.FindProperty("HasRecoverableAccount");
    }

    public override void OnInspectorGUI()
    {
        Draw();
    }

    void Draw()
    {
        EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_firstTimeUser);
            EditorGUILayout.PropertyField(_userName);
            EditorGUILayout.PropertyField(_uniqueIdentifier);
            EditorGUILayout.PropertyField(_blooxCurrency);
            EditorGUILayout.PropertyField(_playerAvatar);
            EditorGUILayout.PropertyField(_playerTheme);
            EditorGUILayout.PropertyField(_playerLevel);
            EditorGUILayout.PropertyField(_playerEXUntilNextLevel);
            EditorGUILayout.PropertyField(_timesPlayedSolo);
            EditorGUILayout.PropertyField(_timesPlayedWipeOut);
            EditorGUILayout.PropertyField(_timesPlayedSurvival);
            EditorGUILayout.PropertyField(_totalModesPlayed);
            EditorGUILayout.PropertyField(_hasRecoverableAccount);
        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();
    }

}
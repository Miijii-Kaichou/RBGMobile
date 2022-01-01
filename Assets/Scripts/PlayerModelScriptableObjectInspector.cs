using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerModel))]
public class PlayerModelScriptableObjectInspector : Editor
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
        EditorGUI.BeginDisabledGroup(true);
        base.OnInspectorGUI();
        EditorGUI.EndDisabledGroup();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
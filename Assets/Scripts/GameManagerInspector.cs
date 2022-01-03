using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR

[CustomEditor(typeof(GameManager))]
public class GameManagerInspector : Editor
{
    const string resetPlayerModeKey = "OptOut.PlayerModel.Reset";
    SerializedProperty playerModel;

    public void OnEnable()
    {
        playerModel = serializedObject.FindProperty("playerModel");
    }

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        EditorGUILayout.BeginHorizontal();
        ColorUtility.TryParseHtmlString("#ff726f", out Color color);
        GUI.color = color;
        bool reset = GUILayout.Button("Reset Player Model");
        if (reset)
        {
            bool confirm = EditorUtility.DisplayDialog("Resetting Player Model",
                "This is a destructive operation, and you may never get this data ever" +
                "again.",
                "Reset", "Cancel", DialogOptOutDecisionType.ForThisMachine, resetPlayerModeKey
                );

            if (confirm)
            {
                (playerModel.objectReferenceValue as PlayerModel).Reset();
                Debug.Log("PlayerModel has Successfully been Resetted.");
            }
        }
        EditorGUILayout.EndHorizontal();
    }
} 
#endif
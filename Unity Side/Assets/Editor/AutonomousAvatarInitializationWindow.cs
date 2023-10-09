using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FacsVersion_AutonomousAvatarInitialization))]
public class AutonomousAvatarInitializationWindow : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        FacsVersion_AutonomousAvatarInitialization alm = target as FacsVersion_AutonomousAvatarInitialization;
        
        GUILayout.FlexibleSpace();

        GUILayout.Space(5f);

        var line = GUILayoutUtility.GetRect(1f, 1f);
        EditorGUI.DrawRect(line, Color.black);
        
        GUILayout.Space(5f);
        
        if (GUILayout.Button("Init Avatar"))
        {
            alm.InitializeAvatar();
            Debug.Log("Avatar initialized");
        }

        GUILayout.Space(5f);

        if (GUILayout.Button("Reset Avatar"))
        {
            alm.ResetAvatar();
            Debug.Log("Avatar reset");
        }
    }
}

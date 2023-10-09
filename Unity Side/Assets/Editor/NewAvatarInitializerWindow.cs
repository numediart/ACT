using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NewAvatarInitializer))]
public class NewAvatarInitializerWindow : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        NewAvatarInitializer nai = target as NewAvatarInitializer;
        
        GUILayout.FlexibleSpace();

        GUILayout.Space(5f);

        var line = GUILayoutUtility.GetRect(1f, 1f);
        EditorGUI.DrawRect(line, Color.black);
        
        GUILayout.Space(5f);
        
        if (GUILayout.Button("Init Avatar"))
        {
            nai.InitNewAvatar();
            Debug.Log("Avatar initialized");
        }

        /*GUILayout.Space(5f);

        if (GUILayout.Button("Reset Avatar"))
        {
            Debug.Log("Avatar reset");
        }*/
    }
}
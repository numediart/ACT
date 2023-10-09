using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AlphaVer_AutonomousLandmarkMaker))]
public class AutonomousLandmarkMakerWindow : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        AlphaVer_AutonomousLandmarkMaker alm = target as AlphaVer_AutonomousLandmarkMaker;
        
        GUILayout.FlexibleSpace();

        GUILayout.Space(5f);

        var line = GUILayoutUtility.GetRect(1f, 1f);
        EditorGUI.DrawRect(line, Color.black);
        
        GUILayout.Space(5f);

        if (GUILayout.Button("Load Landmarks to Json"))
        {
            alm.LoadLandMarksToJson();
            Debug.Log("Loaded Landmarks to Json");
        }
        
        GUILayout.Space(5f);
        
        if (GUILayout.Button("Save Landmarks info to variables"))
        {
            alm.GetAllLandmarksWorldCoordinates();
            Debug.Log("Saved Landmarks info to variables");
        }
        
        GUILayout.Space(5f);
        
        EditorGUI.DrawRect(GUILayoutUtility.GetRect(1f, 1f), Color.black);
        
        GUILayout.Space(5f);

        if (GUILayout.Button("Launch Autonomous Landmark Maker"))
        {
            alm.LaunchLandmarkMaker();
            Debug.Log("Added Landmarks");
        }

        GUILayout.Space(5f);

        if (GUILayout.Button("Reset Autonomous Landmark Maker"))
        {
            alm.ResetLandmarkMaker();
            Debug.Log("Removed Landmarks");
        }
        
    }
}

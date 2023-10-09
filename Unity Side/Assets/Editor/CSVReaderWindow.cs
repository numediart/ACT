using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CsvReader))]
public class CSVReaderWindow : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        CsvReader csvReader = target as CsvReader;
        if (csvReader == null)
        {
            throw new Exception("CSV reader is null");
        }
        
        GUILayout.Space(5f);

        var line = GUILayoutUtility.GetRect(1f, 1f);
        EditorGUI.DrawRect(line, Color.black);
        
        GUILayout.Space(5f);

        if (GUILayout.Button("Read CSV"))
        {
            csvReader.ReadCsv();
            Debug.Log("CSV Read");
        }
        
        GUILayout.Space(5f);

        if (GUILayout.Button("Clear CSV"))
        {
            csvReader.ClearCsv();
            Debug.Log("CSV Cleared");
        }
        
        GUILayout.Space(5f);

        if (GUILayout.Button("Read Frame"))
        {
            csvReader.ReadFrame();
        }
    }
}
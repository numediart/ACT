using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Controls the movement of the head parts, such as eyebrows and eyes.
/// </summary>
public class AlphaVer_HeadController : AlphaVer_PartController
{
    /// <summary>
    /// The global landmarks representing every parts.
    /// </summary>
    [Header("Instances")] public List<AlphaVer_Landmark> Landmarks = new List<AlphaVer_Landmark>();

    private void Start()
    {
        InitiatePartsDictionary();
    }

    /// <summary>
    /// Initializes the dictionary of head parts with their corresponding GameObjects.
    /// </summary>
    protected override void InitiatePartsDictionary()
    {
        Parts = new Dictionary<string, GameObject>();

        foreach (var landmark in Landmarks)
        {
            Parts.Add(landmark.Key, landmark.Reference);
        }
    }
    
    public void ApplyChanges(string key,  Vector3 movement, Vector3 rotation, float timeBeforeRewind)
    {
        StartCoroutine(MoveRotateRewindSpecificKey(key, movement, rotation, timeBeforeRewind));
    }

    #region Functions for Modes

    public void Blink()
    {
        ApplyChanges("lower_lidL", new Vector3(0, 0.05f, 0), Vector3.zero, 0.2f);
        ApplyChanges("upper_lidL",new Vector3(0, -0.15f, 0), Vector3.zero, 0.2f);
        ApplyChanges("lower_lidR", new Vector3(0, 0.05f, 0), Vector3.zero, 0.2f);
        ApplyChanges("upper_lidR",new Vector3(0, -0.15f, 0), Vector3.zero, 0.2f);
    }
    
    #endregion
    
}
    
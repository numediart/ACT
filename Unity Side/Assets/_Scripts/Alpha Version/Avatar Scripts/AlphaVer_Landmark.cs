using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// A landmark object used to identify a game object in the scene.
/// </summary>
public class AlphaVer_Landmark : MonoBehaviour
{
    /// <summary>
    /// The unique key used to identify the landmark.
    /// </summary>
    [HideInInspector] public string Key;

    /// <summary>
    /// The reference to the game object associated with the landmark.
    /// </summary>
    [HideInInspector] public GameObject Reference;

    [FormerlySerializedAs("Localisation")] [HideInInspector] public AlphaVer_Localisation alphaVerLocalisation;
    
    /// <summary>
    /// Called when the landmark is first instantiated.
    /// </summary>
    private void Awake()
    {
        var o = gameObject;
        Reference = o;
        Key = o.name;
        
        FindLocalisation(o.name);
    }

    private void FindLocalisation(string name)
    {
        if (name.Contains("L"))
        {
            alphaVerLocalisation = name.Contains("R") ? AlphaVer_Localisation.ROOT : AlphaVer_Localisation.LEFT;
        }
        else if (name.Contains("R"))
            alphaVerLocalisation = name.Contains("L") ? AlphaVer_Localisation.ROOT : AlphaVer_Localisation.RIGHT;
        else
            alphaVerLocalisation = AlphaVer_Localisation.ROOT;
    }
}


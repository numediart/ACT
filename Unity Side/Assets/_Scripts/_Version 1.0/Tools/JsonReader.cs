using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

public class JsonReader : MonoBehaviour
{
    
    [Header("JSON")] [SerializeField] private TextAsset _actionUnitsJson;
    
    [SerializeField] private TextAsset _blendShapesJson;

    [HideInInspector] public ActionUnitList MyActionUnitList;

    private string _strActionGroupConfigFile;
    private string _strAvatarConfigFile;
    private Dictionary<string, double> _cleanBlendShapeDict;

    public void InitByMainManager()
    {
        ReadActionUnitJson();
        ReadCleanBlendShapeJson();
        
        LoadJsonActionGroupConfigFile();
        LoadJsonAvatarConfigFile();
    }

    public void InitByConfigManger()
    {
        LoadJsonActionGroupConfigFile();
        LoadJsonAvatarConfigFile();
    }

    #region Different JSON readers
    
    private void ReadActionUnitJson()
    {
        // Fill variable
        MyActionUnitList = JsonUtility.FromJson<ActionUnitList>(_actionUnitsJson.text);
        
        // Modify the name of each blend shape in order to match the blend shapes of the avatar
        ChangeKeyToMatchBlendShapes();
        
        // Fill each Action Unit dictionary
        FillActionUnitsDictionaries();
        
        // Fill Action Unit List dictionary
        MyActionUnitList.FillActionUnitListDict();
    }

    private void ReadCleanBlendShapeJson()
    {
        // Fill variable
        _cleanBlendShapeDict = JsonConvert.DeserializeObject<Dictionary<string, double>>(_blendShapesJson.text);
    }

    private void LoadJsonActionGroupConfigFile()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Action Group Configuration File.json");
        _strActionGroupConfigFile = File.ReadAllText(filePath);
    }

    private void LoadJsonAvatarConfigFile()
    {
        string filepath = Path.Combine(Application.streamingAssetsPath, "Avatar Configuration File.json");
        _strAvatarConfigFile = File.ReadAllText(filepath);
    }
    
    public List<AvatarActionGroup> CreateAvatarActionGroupByConfigFile()
    {
        return JsonConvert.DeserializeObject<List<AvatarActionGroup>>(_strActionGroupConfigFile);
    }

    public AvatarConfiguration CreateAvatarConfigByConfigFile()
    {
        return JsonConvert.DeserializeObject<AvatarConfiguration>(_strAvatarConfigFile);
    }
    
    #endregion

    #region Action Unit Json actions

    private void ChangeKeyToMatchBlendShapes()
    {
        foreach (var actionUnit in MyActionUnitList.ActionUnits)
        {
            actionUnit.ClearBlendShapeList();
        }
    }
    
    private void FillActionUnitsDictionaries()
    {
        foreach (var actionUnit in MyActionUnitList.ActionUnits)
        {
            actionUnit.FillBlendShapesDict();
        }
    }

    #endregion

    #region Blend Shapes Json acitons

    public Dictionary<string, double> GetCopyOfCleanBlendShapesDict()
    {
        Dictionary<string, double> copiedDict = new Dictionary<string, double>(_cleanBlendShapeDict.Count);

        foreach (var pair in _cleanBlendShapeDict)
        {
            copiedDict.Add(pair.Key, pair.Value);
        }

        return copiedDict;
    }

    #endregion
    
    #region Configuration

    public void SaveAvatarActionGroupsToJson(List<AvatarActionGroup> avatarActionGroupsToSave)
    {
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "Action Group Configuration File.json"),
            JsonConvert.SerializeObject(avatarActionGroupsToSave, Formatting.Indented));
    }

    public void SaveAvatarParameterToJson(AvatarConfiguration avatarConfigurationToSave)
    {
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "Avatar Configuration File.json"),
            JsonConvert.SerializeObject(avatarConfigurationToSave, Formatting.Indented));
    }
    
    #endregion
}

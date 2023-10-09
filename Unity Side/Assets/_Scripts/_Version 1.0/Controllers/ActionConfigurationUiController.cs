using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionConfigurationUiController : MonoBehaviour
{
    [Header("Instances")] [SerializeField] private ConfigurationAction _configurationActionPrefab;
    [SerializeField] private Transform _mainHudActionContainer;
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private IntensityEditor[] _intensityEditors;
    
    // intern var
    private Dictionary<string, ConfigurationAction> _actionPrefabsByName = new Dictionary<string, ConfigurationAction>();

    #region Configuration Action Prefab
    
    public void UpdateAvatarActionGroupName(string oldAagName, string newAagName)
    {
        if (_actionPrefabsByName.ContainsKey(oldAagName))
        {
            //Update
            var tempValue = _actionPrefabsByName[oldAagName];
            tempValue.ChangeDisplayedName(newAagName);
            
            //Remove key
            _actionPrefabsByName.Remove(oldAagName);
            
            //Add new key
            _actionPrefabsByName.Add(newAagName, tempValue);
        }
        else
        {
            FeedbackManager.Instance.CreateFeedBack(oldAagName + " is not in any dict", FeedbackType.ERROR);
        }
    }

    public void CreateNewAvatarActionGroupUi(string aagName)
    {
        CreateConfigurationAction(aagName);
    }
    
    public void CreateAvatarActionGroupUis(List<AvatarActionGroup> avatarActionGroups)
    {
        foreach (var avatarAction in avatarActionGroups)
        {
           CreateConfigurationAction(avatarAction.Name);
        }
    }

    public void DeleteAvatarActionGroupUi(string aagName)
    {
        // Destroying Game Object
        Destroy(_actionPrefabsByName[aagName].gameObject);
        
        // Removing reference
        _actionPrefabsByName.Remove(aagName);
    }

    private void CreateConfigurationAction(string configActionName)
    {
        ConfigurationAction tempConfigAction = Instantiate(_configurationActionPrefab, _mainHudActionContainer);

        tempConfigAction.ChangeDisplayedName(configActionName);
            
        _actionPrefabsByName.Add(configActionName, tempConfigAction);
    }

    #endregion
    
    #region Input Fields
    
    public void EmptyAllInputs()
    {
        _nameInput.text = "";

        foreach (var intensityEditor in _intensityEditors)
        {
            intensityEditor.EmptyInputs();
        }
    }

    public void FillInputsWithAvatarAction(AvatarActionGroup avatarActionGroup)
    {
        _nameInput.text = avatarActionGroup.Name;

        for (int i = 0; i < avatarActionGroup.AvatarActions.Count; i++)
        {
            _intensityEditors[i].FillInputWithAvatarAction(avatarActionGroup.AvatarActions[i]);
        }
    }

    public bool InputFilledCorrectly()
    {
        bool previousIntensityEditorFilled = true;

        if (_nameInput.text == "")
        {
            FeedbackManager.Instance.CreateFeedBack("Name Input is empty", FeedbackType.WARNING);
            return false;
        }

        if (_intensityEditors[0].CsvInput == "")
        {
            FeedbackManager.Instance.CreateFeedBack("Can't create empty actions", FeedbackType.WARNING);
            return false;
        }
        
        foreach (var intensityEditor in _intensityEditors)
        {
            if (intensityEditor.CsvInput == "" && intensityEditor.AudioInput != "")
            {
                FeedbackManager.Instance.CreateFeedBack("Audio filled without CSV", FeedbackType.WARNING);
                return false;
            }

            //is filled
            if (intensityEditor.CsvInput != "")
            {
                if (!previousIntensityEditorFilled)
                {
                    FeedbackManager.Instance.CreateFeedBack("Filled wrong intensities, please fill the intensities in the right order, from 1 to 5",
                        FeedbackType.WARNING);
                    return false;
                }

            }//is empty
            else
            {
                previousIntensityEditorFilled = false;
            }
        }
        
        return true;
    }
    
    public bool AreFilePathCorrect()
    {
        foreach (var intensityEditor in _intensityEditors)
        {
            // If csv is empty, there is no actions so no test
            if (intensityEditor.CsvInput == "")
                continue;
            
            // Test Csv Input
            if (!File.Exists(intensityEditor.CsvInput))
            {
                FeedbackManager.Instance.CreateFeedBack("Csv file does not exist", FeedbackType.WARNING);
                return false;
            }
            if (Path.GetExtension(intensityEditor.CsvInput) != ".csv")
            {
                FeedbackManager.Instance.CreateFeedBack("Invalid file format : expected a .csv file", FeedbackType.WARNING);
                return false;
            }
            
            // If audio is empty, no need to test it
            if (intensityEditor.AudioInput == "")
                continue;
            
            // Test Audio Input
            if (!File.Exists(intensityEditor.AudioInput))
            {
                FeedbackManager.Instance.CreateFeedBack("Audio file does not exist", FeedbackType.WARNING);
                return false;
            }
            if (Path.GetExtension(intensityEditor.AudioInput) != ".wav")
            {
                FeedbackManager.Instance.CreateFeedBack("Invalid file format : expected a .wav file", FeedbackType.WARNING);
                return false;
            }
            
        }
        
        return true;
    }
    
    #endregion

    public AvatarActionGroup GetInputInfoConvertedInAvatarActionGroup()
    {
        AvatarActionGroup avatarActionGroup = new AvatarActionGroup();
        List<AvatarAction> avatarActions = new List<AvatarAction>();

        avatarActionGroup.Name = _nameInput.text;
        
        foreach (var intensityEditor in _intensityEditors)
        {
            if (intensityEditor.CsvInput == "")
                break;
            
            AvatarAction tempAvatarAction = new AvatarAction();
            
            tempAvatarAction.CsvFilePath = intensityEditor.CsvInput;
            tempAvatarAction.AudioFilePath = intensityEditor.AudioInput;
            
            avatarActions.Add(tempAvatarAction);
        }

        avatarActionGroup.AvatarActions = avatarActions;

        return avatarActionGroup;
    }
}

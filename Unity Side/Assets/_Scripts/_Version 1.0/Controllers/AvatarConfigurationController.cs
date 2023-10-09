using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarConfigurationController : MonoBehaviour
{
    [Header("Instances")] [SerializeField] private AvatarConfigurationUiController _ui;

    //intern var
    private JsonReader _jsonReaderReference;
    private AvatarConfiguration _avatarConfiguration;

    private void Start()
    {
        _jsonReaderReference = ConfigurationManager.Instance.JsonReader;
        
        _avatarConfiguration = _jsonReaderReference.CreateAvatarConfigByConfigFile();
        
        _ui.Init(_avatarConfiguration);
    }

    public void ConfirmChanges()
    {
        var acChecker = _ui.ConfirmChangesEvent();

        if (acChecker != null)
        {
            _avatarConfiguration = acChecker;
            SaveAvatarConfigurationToJson();
            ConfigurationManager.Instance.LoadMainConfiguration();
        }
        else
        {
            //Feedback
            FeedbackManager.Instance.CreateFeedBack("Error : please fill every input", FeedbackType.ERROR);
        }
    }

    public void SaveAvatarConfigurationToJson()
    {
        _jsonReaderReference.SaveAvatarParameterToJson(_avatarConfiguration);
        FeedbackManager.Instance.CreateFeedBack("Configuration File saved", FeedbackType.SUCCESS);
    }
}

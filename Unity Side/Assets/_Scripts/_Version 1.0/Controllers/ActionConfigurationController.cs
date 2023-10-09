using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ActionConfigurationController : MonoBehaviour
{
    [Header("Instances")]
    [SerializeField] private ActionConfigurationUiController _actionConfigurationUiController;
    [SerializeField] private GameObject _mainHudGo;
    [SerializeField] private GameObject _createAndEditHudGo;

    //Intern var
    private List<AvatarActionGroup> _avatarActionGroups;
    private Dictionary<string, AvatarActionGroup> _avatarActionGroupsByName;
    private AvatarActionGroup _currentAvatarActionGroup;
    private JsonReader _jsonReaderReference;

    private void Start()
    {
        _jsonReaderReference = ConfigurationManager.Instance.JsonReader;
        
        _jsonReaderReference.InitByConfigManger();
        
        _avatarActionGroupsByName = new Dictionary<string, AvatarActionGroup>();
        
        _avatarActionGroups = _jsonReaderReference.CreateAvatarActionGroupByConfigFile();

        foreach (var avatarActionGroup in _avatarActionGroups)
        {
            _avatarActionGroupsByName.Add(avatarActionGroup.Name, avatarActionGroup);
        }
        
        _actionConfigurationUiController.CreateAvatarActionGroupUis(_avatarActionGroups);
    }
    
    // Called Externally
    public void ConfigurationActionSelectedEvent(string actionName)
    {
        // Show edit tab with personalized inputs
        ShowCreateAndEditHud();
        _actionConfigurationUiController.EmptyAllInputs();
        _currentAvatarActionGroup = _avatarActionGroupsByName[actionName];
        _actionConfigurationUiController.FillInputsWithAvatarAction(_avatarActionGroupsByName[actionName]);
    }

    // Listener on "arrow" when opening an action
    public void BackToMainHud()
    {
        _actionConfigurationUiController.EmptyAllInputs();
        _currentAvatarActionGroup = null;
        ShowMainHud();
    }

    // Listener on "+"
    public void CreateNewActionEvent()
    {
        // Show edit tab with emptied inputs
        ShowCreateAndEditHud();
        _actionConfigurationUiController.EmptyAllInputs();
        _currentAvatarActionGroup = null;
    }

    // Listener on "Confirm"
    public void ConfirmChanges()
    {
        // if confirmed, apply changes and back to main hud
        if (_actionConfigurationUiController.InputFilledCorrectly() && _actionConfigurationUiController.AreFilePathCorrect())
        {
            AvatarActionGroup avatarActionGroup = _actionConfigurationUiController.GetInputInfoConvertedInAvatarActionGroup();

            // Modify if already exists
            if (_currentAvatarActionGroup != null)
            {
                // If necessary, change key
                if (_currentAvatarActionGroup.Name != avatarActionGroup.Name)
                {
                    if (_avatarActionGroupsByName.ContainsKey(avatarActionGroup.Name))
                    {
                        ActionGroupKeyNotAvailableEvent();
                        return;
                    }
                    
                    //Removing old key
                    _avatarActionGroupsByName.Remove(_currentAvatarActionGroup.Name);
                    
                    //Adding new key
                    _avatarActionGroupsByName.Add(avatarActionGroup.Name, avatarActionGroup);
                    
                    //Trigger ui for name change
                    _actionConfigurationUiController.UpdateAvatarActionGroupName(_currentAvatarActionGroup.Name, avatarActionGroup.Name);
                }
                else
                {
                    _avatarActionGroupsByName[avatarActionGroup.Name] = avatarActionGroup;
                }
            }
            // Create if new
            else
            {
                if (_avatarActionGroupsByName.ContainsKey(avatarActionGroup.Name))
                {
                    ActionGroupKeyNotAvailableEvent();
                    return;
                }
                
                _avatarActionGroupsByName.Add(avatarActionGroup.Name, avatarActionGroup);
                
                _actionConfigurationUiController.CreateNewAvatarActionGroupUi(avatarActionGroup.Name);
            }

            _currentAvatarActionGroup = null;
            _actionConfigurationUiController.EmptyAllInputs();
            ShowMainHud();
            
            FeedbackManager.Instance.CreateFeedBack($"{avatarActionGroup.Name} : successfully created", FeedbackType.SUCCESS);
        }
    }
    
    // Listener on "Trash"
    public void DeleteAvatarActionGroup()
    {
        // If user is editing an existing action, he can delete this one
        if (_currentAvatarActionGroup != null)
        {
            string currentAagName = _currentAvatarActionGroup.Name;

            _currentAvatarActionGroup = null;

            _avatarActionGroupsByName.Remove(currentAagName);
            _actionConfigurationUiController.DeleteAvatarActionGroupUi(currentAagName);
            
            FeedbackManager.Instance.CreateFeedBack($"{currentAagName} : successfully deleted", FeedbackType.SUCCESS);
        }
        
        // If not editing, it's like using the listener of the arrow icon
        BackToMainHud();
    }
    
    public void SaveAvatarActionGroupsToJson()
    {
        _avatarActionGroups = new List<AvatarActionGroup>();

        foreach (var pair in _avatarActionGroupsByName)
        {
            _avatarActionGroups.Add(pair.Value);
        }
        
        _jsonReaderReference.SaveAvatarActionGroupsToJson(_avatarActionGroups);
        
        FeedbackManager.Instance.CreateFeedBack("Configuration File saved", FeedbackType.SUCCESS);
    }

    private void ActionGroupKeyNotAvailableEvent()
    {
        FeedbackManager.Instance.CreateFeedBack("You are trying to add an already existing key", FeedbackType.ERROR);
    }
    
    private void ShowMainHud()
    {
        _createAndEditHudGo.SetActive(false);
        _mainHudGo.SetActive(true);
    }

    private void ShowCreateAndEditHud()
    {
        _mainHudGo.SetActive(false);
        _createAndEditHudGo.SetActive(true);
    }
}

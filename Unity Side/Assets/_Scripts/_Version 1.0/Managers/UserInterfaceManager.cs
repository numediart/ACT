using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RockVR.Video;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    [Header("Global Interface")] [SerializeField] private KeyCode _toggleInterfaceKey;
    [SerializeField] private GameObject _adminHud;
    
    #region Left ADMIN HUD
    [Header("Interface Actions")] [SerializeField] private KeyCode _playPauseKey;
    [SerializeField] private KeyCode _skipKey;
    [SerializeField] private KeyCode _backKey;

    [Header("Actions Control")] [SerializeField] private GameObject _actionInControlsPrefab;
    [SerializeField] private Transform _controlActionButtonsScrollView;
    [SerializeField] private Transform _controlActionButtonsContent; 
    [SerializeField] private Transform _controlIntensityButtonsScrollView;
    [SerializeField] private Transform _intensityButtons;

    [Header("Actions Queue")] [SerializeField] private GameObject _actionInQueuePrefab;
    [SerializeField] private Transform _actionInQueueContent;
    [SerializeField] private Color _activeActionColor;
    #endregion
    
    #region Right ADMIN HUD

    [Header("Joan's Parameters")] [SerializeField] private Slider _headRotSlider;
    [SerializeField] private Slider _neckRotSlider;
    
    #endregion
    
    private List<AvatarQueueUI> _allActionsInQueue = new List<AvatarQueueUI>();
    private AvatarQueueUI _currentAvatarQueueUI;

    // test
    private bool _recordOn = false;

    public void Init()
    {
        if (NetworkManager.Instance.PioconnectionRoom == null)
            UpdateSlidersValues();
        
        HideAllIntensities();
    }

    private void Update()
    {
        if (Input.GetKeyDown(_toggleInterfaceKey))
        { 
            ToggleAdminHudVisibility();
        }
        else if (Input.GetKeyDown(_playPauseKey))
        {
            TriggerPlayPauseAction();
        }
        else if (Input.GetKeyDown(_skipKey))
        {
            TriggerSkipAction();
        }
        else if (Input.GetKeyDown(_backKey))
        {
            TriggerBackAction();
        }
    }

    #region Interface actions

    private void TriggerPlayPauseAction()
    {
        MainManager.Instance.PauseStartActionExecution();
    }

    private void TriggerSkipAction()
    {
        MainManager.Instance.SkipToNextAction();
    }

    private void TriggerBackAction()
    {
        MainManager.Instance.BackToLastAction();
    }

    public void ToggleAdminHudVisibility()
    {
        _adminHud.SetActive(!_adminHud.activeSelf);
    }

    #endregion
    
    #region Actions Control

    public void CreateActionGroupInControls(string actionName, int nbIntensities, int actionGroupIndex)
    {
        GameObject go = Instantiate(_actionInControlsPrefab, _controlActionButtonsContent);
        
        AvatarControlUI avatarControlUI = go.GetComponent<AvatarControlUI>();

        avatarControlUI.NbIntensities = nbIntensities;
        avatarControlUI.ActionIndex = actionGroupIndex;
        avatarControlUI.ModifyActionName(actionName.Trim());
    }

    public void LoadIntensitiesForActionGroup(int nbIntensities)
    {
        _controlActionButtonsScrollView.gameObject.SetActive(false);
        
        _controlIntensityButtonsScrollView.gameObject.SetActive(true);
        
        for (int i = 0; i < nbIntensities; i++)
        {
            _intensityButtons.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void HideAllIntensities()
    {
        for (int i = 0; i < _intensityButtons.childCount - 1; i++)
        {
            _intensityButtons.GetChild(i).gameObject.SetActive(false);
        }
        
        _controlIntensityButtonsScrollView.gameObject.SetActive(false);
        
        _controlActionButtonsScrollView.gameObject.SetActive(true);
    }

    #endregion
    
    #region Actions Queue

    public void CreateActionInQueue(string actionName, int actionIntensity)
    {
        GameObject go = Instantiate(_actionInQueuePrefab, _actionInQueueContent);
        
        AvatarQueueUI avatarQueueUI = go.GetComponent<AvatarQueueUI>();

        _allActionsInQueue.Add(avatarQueueUI);

        StringBuilder sb = new StringBuilder();
        sb.Append("Intensity");
        sb.Append(" ");
        sb.Append(actionIntensity);
        
        avatarQueueUI.ModifyActionNb(_allActionsInQueue.Count);
        avatarQueueUI.ModifyActionName(actionName.Trim());
        avatarQueueUI.ModifyActionIntensity(sb.ToString());
        
        avatarQueueUI.SetActionIndex(_allActionsInQueue.Count - 1);
    }

    public void SetActionToCurrent(int actionIndex)
    {
        if (_currentAvatarQueueUI != null)
        {
            _currentAvatarQueueUI.ModifyBackgroundColor(Color.white);
        }

        _currentAvatarQueueUI = _allActionsInQueue[actionIndex];
        _currentAvatarQueueUI.ModifyBackgroundColor(_activeActionColor);
    }

    public void NoCurrentActionEvent()
    {
        if (_currentAvatarQueueUI != null)
        {
            _currentAvatarQueueUI.ModifyBackgroundColor(Color.white);
            _currentAvatarQueueUI = null;
        }
    }
    
    public void DeleteActionInQueueByIndex(int actionIndex)
    {
        Destroy(_allActionsInQueue[actionIndex].gameObject);
        _allActionsInQueue.RemoveAt(actionIndex);
    }

    public void UpdateActionsInQueueFromIndex(int actionIndex)
    {
        for (int i = actionIndex; i < _allActionsInQueue.Count; i++)
        {
            _allActionsInQueue[i].ModifyActionNb(i + 1);
            _allActionsInQueue[i].SetActionIndex(i);
        }
    }

    #endregion
    
    #region Joan's Parameters
    
    // Dynamic listeners
    public void OnHeadRotChange(float value)
    {
        MainManager.Instance.HeadPoseController.HeadRotCorrection = new Vector3(value, 0, 0);
    }

    public void OnNeckRotChange(float value)
    {
        MainManager.Instance.HeadPoseController.NeckRotCorrection = new Vector3(value, 0, 0);
    }

    public void OnReset()
    {
        MainManager.Instance.ResetAvatarToNeutralPosition();
    }
    
    private void UpdateSlidersValues()
    {
        _headRotSlider.value = MainManager.Instance.HeadPoseController.HeadRotCorrection.x;
        _neckRotSlider.value = MainManager.Instance.HeadPoseController.NeckRotCorrection.x;
    }
    
    #endregion
}

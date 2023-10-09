using System;
using System.Collections;
using System.Threading.Tasks;
using RockVR.Video;
using UnityEngine;
using UnityEngine.Serialization;

public class MainManager : MonoBehaviour
{
    #region Singleton
    public static MainManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    #endregion

    #region Public Fields
    [Header("Managers")] public UserInterfaceManager UIManager;
    
    [Header("Tools")] 
    public CsvReader CsvReader;
    public JsonReader JsonReader;

    [Header("Controllers")] 
    public BlendShapesController BlendShapesController;
    public HeadPoseController HeadPoseController;
    public AudioController AudioController;
    public VideoCaptureCtrl VideoCaptureCtrlRef;

    // make those serializable ?
    [Header("Actions")] 
    public AvatarActionGroupList avatarActionGroupList;
    public AvatarActionQueue ActionQueue;

    [Header("Parameters")] public NetworkMode NetworkMode;
    #endregion

    #region Private Fields

    private AvatarAction _currentAvatarAction;
    private AvatarActionGroup _currentAvatarActionGroupSelected;
    private AvatarConfiguration _avatarConfiguration;
    private bool _isRecording;
    [SerializeField] private float _transitionDuration;
    [SerializeField] private bool _isOn = false;
    [SerializeField] private bool _fullExpressionOn;
    
    #endregion

    #region Getters

    public float TransitionDuration
    {
        get => _transitionDuration;
        private set => _transitionDuration = value;
    }

    #endregion

    private void Start()
    {
        // Tools
        JsonReader.InitByMainManager();
        
        // Avatar configuration
        _avatarConfiguration = JsonReader.CreateAvatarConfigByConfigFile();
        _transitionDuration = _avatarConfiguration.TransitionDelay;
        HeadPoseController.HeadRotCorrection = _avatarConfiguration.HeadRotCorrection;
        HeadPoseController.NeckRotCorrection = _avatarConfiguration.NeckRotCorrection;
        _fullExpressionOn = _avatarConfiguration.FullExpressionOn;
        
        // Managers
        if (UIManager != null)
            UIManager.Init();

        // Controllers
        BlendShapesController.Init();
        HeadPoseController.Init();
        if (AudioController != null)
            AudioController.Init();
        
        // Helpers
        avatarActionGroupList.Init();
        ActionQueue = new AvatarActionQueue();
    }

    private void Update()
    {
        if (_isOn)
        {
            FrameExecution();
        }
    }

    // Called each frame
    private void FrameExecution()
    {
        if (_currentAvatarAction != null)
            ExecuteActionForCurrentFrame(_currentAvatarAction);
        else
        {
            _isOn = false;
        }
    }

    #region Avatar Actions Queue

    // Used as a listener of button in UI
    public void PauseStartActionExecution()
    {
        // Start
        //TODO : change the boolean to an enum to differentiate the start and the resume function
        if (!_isOn)
        {
            StartAvatarActionQueueExecution();
        }

        // Pause
        else
        {
            _isOn = false;
            
            if (_currentAvatarAction != null && _currentAvatarAction.ContainsAudio)
                AudioController.PauseAudioClip();
        }
    }

    // Used as a listener of button in UI
    public void StartAndRecordQueue()
    {
        if (_isOn)
            return;

        _isRecording = true;

        StartRecordingAsync(); 
    }
    
    #region Record Manager
    private async Task StartRecordingAsync()
    {
        if (ActionQueue.GetCurrentAvatarAction() == null)
            return;
        
        UIManager.ToggleAdminHudVisibility();
        
        await VideoCaptureCtrlRef.StartCapture();
        
        StartAvatarActionQueueExecution();
    }

    private async Task StopRecordingAsync()
    {
        await VideoCaptureCtrlRef.StopCapture();

        _isRecording = false;
        
        UIManager.ToggleAdminHudVisibility();
        
        UIManager.NoCurrentActionEvent();
    }
    #endregion
    
    private void StartAvatarActionQueueExecution()
    {
        UpdateCurrentAvatarAction();
        MakeTransitionBetweenActions();
        if (_currentAvatarAction != null && _currentAvatarAction.ContainsAudio)
        {
            AudioController.ResumeAudioClip();
        }
    }

    // Used as a listener of button in UI
    public void SkipToNextAction()
    {
        FinishCurrentActionIfNotNull();
        
        ForceActionEndBySkip(true);
    }
    
    // Used as a listener of button in UI
    public void BackToLastAction()
    {
        FinishCurrentActionIfNotNull();
        
        ForceActionEndBySkip(false);
    }
    
    private void FinishCurrentActionIfNotNull()
    {
        if (_currentAvatarAction != null)
            _currentAvatarAction.ForceFinish();
    }
    
    private void UpdateCurrentAvatarAction()
    {
        _currentAvatarAction = ActionQueue.GetCurrentAvatarAction();
        
        if (_currentAvatarAction != null)
        {
            PrepareAction(_currentAvatarAction);
            UIManager.SetActionToCurrent(ActionQueue.CurrentActionIndex);
        }
        else
        {
            if (_isRecording)
            {
                StopRecordingAsync();
            }
            else
            {
                UIManager.NoCurrentActionEvent();
            }
        }
    }

    //TODO : fix targetFrameRate
    private void PrepareAction(AvatarAction action)
    {
        Application.targetFrameRate = action.FrameManager.Fps;
        
        if (!_fullExpressionOn)
            action.FrameManager.SkipFramesForTransition();
    }
    
    private void ExecuteActionForCurrentFrame(AvatarAction action)
    {
        Frame frameExecuted = action.ActionFrameList.Frames[action.FrameManager.FrameNb];

        // Audio
        if (action.ContainsAudio && action.FrameManager.FrameNb == 0)
        {
            AudioController.PlayAudioClip(action.Audio);
        }
        
        // Visual
        BlendShapesController.BlendShapeUpdateForFrame(frameExecuted);
        HeadPoseController.HeadPoseUpdateByFrame(frameExecuted);

        // End
        if (!_fullExpressionOn)
            action.FrameManager.FrameExecuted(ActionQueue.HasOtherActionAfterCurrent());
        else
            action.FrameManager.FrameExecuted(false);
    }

    // Called when an action is finished 
    public void EndOfActionEvent()
    {
        ActionQueue.CurrentAvatarActionFinished();
        
        if (_currentAvatarAction.ContainsAudio)
            AudioController.StopAudioClip();

        UpdateCurrentAvatarAction();
        
        MakeTransitionBetweenActions();
    }
    
    private void MakeTransitionBetweenActions()
    {
        if (_currentAvatarAction != null)
        {
            _isOn = false;

            Frame frameToBeExecutedNext;
            
            // if we just changed the action or we paused just before the end, we transition to the same frame we are at
            if (_currentAvatarAction.FrameManager.FrameNb == 0 || 
                _currentAvatarAction.FrameManager.FrameNb >= _currentAvatarAction.FrameManager.MaxFrameNb - 1)
            {
                frameToBeExecutedNext =
                    _currentAvatarAction.ActionFrameList.Frames[_currentAvatarAction.FrameManager.FrameNb];
            }

            // if we already passed the current frame we go to the next one
            else
            {
                frameToBeExecutedNext =
                    _currentAvatarAction.ActionFrameList.Frames[_currentAvatarAction.FrameManager.FrameNb + 1];
            }
            
            
            BlendShapesController.TransitionBetweenFrames(frameToBeExecutedNext, _transitionDuration);
            HeadPoseController.TransitionToPoseOverTime(frameToBeExecutedNext.PoseDict, _transitionDuration);
            
            StartCoroutine(TransitionWaiter());
        }
    }

    private IEnumerator TransitionWaiter()
    {
        yield return new WaitForSeconds(_transitionDuration);
        _isOn = true;
    }

    // Called when action is ended prematurely
    private void ForceActionEndBySkip(bool isSkipped)
    {
        if (isSkipped)
            ActionQueue.CurrentAvatarActionSkipped();
        else
            ActionQueue.CurrentAvatarActionBacked();
        
        UpdateCurrentAvatarAction();
        
        MakeTransitionBetweenActions();
    }

    #endregion

    #region Avatar Actions Controls

    public void ActionListCreationFinishedEvent()
    {
        for (int i = 0; i < avatarActionGroupList.AvatarActionGroups.Count; i++)
        {
            UIManager.CreateActionGroupInControls(avatarActionGroupList.AvatarActionGroups[i].Name, 
                avatarActionGroupList.AvatarActionGroups[i].AvatarActions.Count, i);
        }
    }
    
    public void AvatarActionSelected(int index, int nbIntensities)
    {
        _currentAvatarActionGroupSelected = avatarActionGroupList.AvatarActionGroups[index];
        
        UIManager.LoadIntensitiesForActionGroup(nbIntensities);
    }

    public void AvatarActionIntensitySelected(int intensityId)
    {
        //We have a current action selected and intensity, we can add the action to queue
        
        ActionQueue.AddActionToQueue(_currentAvatarActionGroupSelected.AvatarActions[intensityId]);
        
        UIManager.CreateActionInQueue(_currentAvatarActionGroupSelected.Name, intensityId + 1);

        _currentAvatarActionGroupSelected = null;
        
        UIManager.HideAllIntensities();
    }

    // Listener on "cancel" button in Intensity UI
    public void CancelAvatarAction()
    {
        _currentAvatarActionGroupSelected = null;
        
        UIManager.HideAllIntensities();
    }

    public void ActionDeletedFromQueueEvent(int actionIndex)
    {
        ActionQueue.RemoveActionFromQueueByIndex(actionIndex);
        
        UIManager.DeleteActionInQueueByIndex(actionIndex);
        
        UIManager.UpdateActionsInQueueFromIndex(actionIndex);
        
        FeedbackManager.Instance.CreateFeedBack("Action Deleted", FeedbackType.SUCCESS);
    }

    #endregion
    
    #region Joan's Parameters

    public void ResetAvatarToNeutralPosition()
    {
        if (_isOn)
            return;
        
        BlendShapesController.MakeBlendShapesResetOverTime(_transitionDuration);
        HeadPoseController.MakePoseResetOverTime(_transitionDuration);
    }
    
    #endregion
}

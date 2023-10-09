using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Represents the manager of every other script in the Scene.
/// </summary>
public class AlphaVer_MainScript : MonoBehaviour
{
    /// <summary>
    /// Singleton, created early and not used yet
    /// </summary>
    public static AlphaVer_MainScript s_instance;
    public void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
    }
    
    /// <summary>
    /// Instance of JsonGetter script
    /// </summary>
    [FormerlySerializedAs("JsonGetter")] [Header("Instance")] public AlphaVer_JsonGetter alphaVerJsonGetter;

    [FormerlySerializedAs("JsonWritter")] public BetaVer_JsonWritter betaVerJsonWritter;
    /// <summary>
    /// Instance of AvatarController script
    /// </summary>
    [FormerlySerializedAs("AvatarController")] public AlphaVer_AvatarController alphaVerAvatarController;
    /// <summary>
    /// Instance of AudioManager script
    /// </summary>
    [FormerlySerializedAs("AudioManager")] public AlphaVer_AudioManager alphaVerAudioManager;

    [FormerlySerializedAs("AutonomousLandmarkMaker")] public AlphaVer_AutonomousLandmarkMaker alphaVerAutonomousLandmarkMaker;

    [FormerlySerializedAs("UIManager")] public AlphaVer_UIManager alphaVerUIManager;

    /// <summary>
    /// Time elapsed between 2 json updates
    /// </summary>
    [Header("Options")] public float TimeBetweenMoves;
    [FormerlySerializedAs("Mode")] public AlphaVer_Mode alphaVerMode;
    
    // Intern var
    private float _moveClock;

    private void Start()
    {
        _moveClock = 0;
        UpdateStatus();
        alphaVerUIManager.InitiateHudManual(alphaVerAvatarController.ManualEvents);
    }

    private void Update()
    {
        _moveClock += Time.deltaTime;
        if (_moveClock >= TimeBetweenMoves)
        {
            LaunchASequence();
            _moveClock = 0;
        }
    }
    
    private void LaunchASequence()
    {
        switch (alphaVerMode)
        {
            case AlphaVer_Mode.DATA:
                DataSequence();
                break;
            case AlphaVer_Mode.BLINKING:
                BlinkSequence();
                break;
            case AlphaVer_Mode.IDLE:
                IdleSequence();
                break;
        }
    }

    private void UpdateStatus()
    {
        switch (alphaVerMode)
        {
            case AlphaVer_Mode.DATA:
                alphaVerUIManager.ChangeStatus("Data");
                break;
            case AlphaVer_Mode.MANUAL:
                alphaVerUIManager.ChangeStatus("Manual");
                break;
            case AlphaVer_Mode.PAUSED:
                alphaVerUIManager.ChangeStatus("Paused");
                break;
            case AlphaVer_Mode.BLINKING:
                alphaVerUIManager.ChangeStatus("Blink");
                break;
            case AlphaVer_Mode.IDLE:
                alphaVerUIManager.ChangeStatus("Idle");
                break;
            default:
                alphaVerUIManager.ChangeStatus("Default");
                break;
        }
    }

    #region Listeners for Mode Changes
    
    public void ChangeToIdleMode()
    {
        alphaVerMode = AlphaVer_Mode.IDLE;
        UpdateStatus();
        alphaVerUIManager.HideHudManual();
    }
    
    public void ChangeToPausedMode()
    {
        alphaVerMode = AlphaVer_Mode.PAUSED;
        UpdateStatus();
        alphaVerUIManager.HideHudManual();
    }

    public void ChangeToDataMode()
    {
        alphaVerMode = AlphaVer_Mode.DATA;
        UpdateStatus();
        alphaVerUIManager.HideHudManual();
    }

    public void ChangeToManualMode()
    {
        alphaVerMode = AlphaVer_Mode.MANUAL;
        UpdateStatus();
        alphaVerUIManager.ShowHudManual();
    }

    public void DisplayEvents()
    {
        alphaVerMode = AlphaVer_Mode.DATA;
        UpdateStatus();
        alphaVerUIManager.ShowHudManual();
    }
    
    public void ChangeToBlinkingMode()
    {
        alphaVerMode = AlphaVer_Mode.BLINKING;
        UpdateStatus();
        alphaVerUIManager.HideHudManual();
    }
    
    #endregion
    
    #region Data Sequence
    
    private void DataSequence()
    {
        // Update the JSON data.
        alphaVerJsonGetter.UpdateJson();
    
        // Start the coroutine for executing head orders.
        StartCoroutine(ExecuteHeadOrders());
    
        // Start the coroutine for playing order sounds.
        StartCoroutine(PlayOrderSound());
    }
    
    /// <summary>
    /// Executes head movement orders based on the JSON data.
    /// </summary>
    private IEnumerator ExecuteHeadOrders()
    {
        // Iterate through each move order in the JSON data.
        foreach (var order in alphaVerJsonGetter.MyOrderList.MoveOrders)
        {
            // Call the HeadModificationEvent function in the AvatarController script.
            alphaVerAvatarController.HeadModificationEvent(order.LandmarkKey, order.MovementCoefficient, order.RotationCoefficient, order.TimeBeforeRewind);
        }
    
        // End the coroutine.
        yield break;
    }
    
    /// <summary>
    /// Plays the audio clip specified in the JSON data.
    /// </summary>
    private IEnumerator PlayOrderSound()
    {
        // Call the PlaySound function in the AudioManager script with the audio clip name from the JSON data.
        alphaVerAudioManager.PlaySound(alphaVerJsonGetter.MyOrderList.AudioClipName);
    
        // End the coroutine.
        yield break;
    }

    #endregion

    #region Blink Sequence

    private void BlinkSequence()
    {
        alphaVerAvatarController.BlinkEvent();
    }

    #endregion
    
    #region Idle Sequence

    private void IdleSequence()
    {
        
    }

    #endregion
}

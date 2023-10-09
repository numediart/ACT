using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// A class for controlling an avatar's head and triggering head movement events.
/// </summary>
public class AlphaVer_AvatarController : MonoBehaviour
{
    [HideInInspector] public List<AlphaVer_ManualEvent> ManualEvents;

    /// <summary>
    /// The controller for the avatar's head.
    /// </summary>
    [FormerlySerializedAs("HeadController")] [Header("Parts")] public AlphaVer_HeadController alphaVerHeadController;

    /// <summary>
    /// Triggers a head movement event for the specified key, movement, and time before rewinding.
    /// </summary>
    /// <param name="key">The key for the head movement.</param>
    /// <param name="movement">The movement for the head movement.</param>
    /// <param name="rotation">The rotation for the head movement. </param>
    /// <param name="timeBeforeRewind">The time to wait before rewinding the head movement.</param>
    public void HeadModificationEvent(string key, Vector3 movement, Vector3 rotation, float timeBeforeRewind)
    {
        alphaVerHeadController.ApplyChanges(key, movement, rotation, timeBeforeRewind);
    }

    #region Special movement event
    
    /// <summary>
    /// Raises a surprise event by moving all eyebrows up.
    /// </summary>
    /*public void SurpriseEvent()
    {
        HeadController.MoveAllEyebrowsUp();
    }

    public void WonderingEventLeft()
    {
        HeadController.MoveLeftEyebrowUp();
    }

    public void WonderingEventRight()
    {
        HeadController.MoveRightEyebrowUp();
    }

    public void SquintEvent()
    {
        HeadController.RotateLeftEyeRight();
        HeadController.RotateRightEyeLeft();
    }

    public void PufferFishEvent()
    {
        HeadController.MoveCheeksToPuffer();
    }

    public void MegamindEvent()
    {
        HeadController.MoveForeheadsToMegamind();
    }

    public void WowEvent()
    {
        HeadController.JawWowEvent();
    }

    public void BogdanovEvent()
    {
        HeadController.ChinToBogdanov();
    }

    public void CringedEvent()
    {
        HeadController.CringedCommissures();
    }

    public void HuhEvent()
    {
        HeadController.LipsHuh();
    }*/

    public void BlinkEvent()
    {
        alphaVerHeadController.Blink();
    }

    public void IdleEvent()
    {
        
    }

    #endregion

    #region Move and rotate tests
    
    public void RotatePartEvent()
    {
        
    }

    public void PartEvent()
    {
        
    }
    
    #endregion
}

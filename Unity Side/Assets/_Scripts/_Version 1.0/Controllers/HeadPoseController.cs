using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class HeadPoseController : MonoBehaviour
{
    // Needs to be public for NewAvatarInitializer.cs
    #region Public fields
    [Header("Instances")] public Transform HeadJoint;
    public Transform NeckJoint;

    [Header("Parameters")] public Vector3 HeadRotCorrection = new Vector3(-23.86f, 0, 0);
    public Vector3 NeckRotCorrection = new Vector3(32.928f, 0, 0); // + 32.928f normally but 18 for my correction
    #endregion

    #region Private Fields

    private float _headMultiplier = 0.65f;
    private float _neckMultiplier = 0.35f;

    private Quaternion _neutralHeadValue;
    private Quaternion _neutralNeckValue;

    #endregion

    private void Start()
    {
        
    }

    public void Init()
    {
        _neutralHeadValue = HeadJoint.localRotation;
        _neutralNeckValue = NeckJoint.localRotation;
    }

    // Function used in MainManager to Update the Head & Neck rotation according to a Frame
    // The same function is called when the admin is moving the avatar for the user
    public void HeadPoseUpdateByFrame(Frame frame)
    {
        MakeServerRequestForHeadPose(frame.PoseDict);

        Vector3 rot = ExtractRotationFromDict(frame.PoseDict);
        
        // Make HeadPos Change Instantly
        HeadJoint.localRotation = CalculateQuaternionForPose(rot, _headMultiplier, HeadRotCorrection);
        NeckJoint.localRotation = CalculateQuaternionForPose(rot, _neckMultiplier, NeckRotCorrection);
    }

    private Vector3 ExtractRotationFromDict(Dictionary<string, double> newPose)
    {
        // Do the calculation before casting so you lose less data
        double x = newPose["pose_Rx"] * Mathf.Rad2Deg, 
            y = newPose["pose_Ry"] * Mathf.Rad2Deg,
            z = newPose["pose_Rz"] * Mathf.Rad2Deg;

        return new Vector3((float) x, (float) y, (float) z);
    }

    private Quaternion CalculateQuaternionForPose(Vector3 rotation, float multiplier, Vector3 correction)
    {
        return Quaternion.Euler((rotation * multiplier) + correction);
    }

    public void MakePoseResetOverTime(float transitionDuration)
    {
        StartCoroutine(
            MakeHeadPosChangeTowardValueOverTime(
                HeadJoint.localRotation,
                NeckJoint.localRotation,
                _neutralHeadValue,
                _neutralNeckValue,
                transitionDuration
            )
        );
    }
    
    /// <summary>
    /// The goal is to change Head pose from current position to future position smoothly
    /// This will not be used between every frame (we assume the video tracking device is doing a nice job), but between frames from different actions
    /// Example : at the end of an action, we make a transition from the last frame of this action to the first frame of the next action if it exists.
    /// </summary>
    /// <param name="futurePose"></param>
    /// <param name="transitionDuration"></param>
    public void TransitionToPoseOverTime(Dictionary<string, double> futurePose, float transitionDuration)
    {
        Vector3 rot = ExtractRotationFromDict(futurePose);

        // Local
        MakeRotTransition(rot, transitionDuration);
        
        // Server
        SendHeadPoseTransitionToUser(rot, transitionDuration);
    }

    public void MakeRotTransition(Vector3 rot, float transitionDuration)
    {
        Quaternion futureHeadPose = CalculateQuaternionForPose(rot, _headMultiplier, HeadRotCorrection);
        Quaternion futureNeckPose = CalculateQuaternionForPose(rot, _neckMultiplier, NeckRotCorrection);

        StartCoroutine(
            MakeHeadPosChangeTowardValueOverTime(
                HeadJoint.localRotation,
                NeckJoint.localRotation,
                futureHeadPose,
                futureNeckPose,
                transitionDuration
            )
        );

    }
    
    private IEnumerator MakeHeadPosChangeTowardValueOverTime(Quaternion headStartValue, Quaternion neckStartValue, Quaternion headEndValue, Quaternion neckEndValue, float duration)
    {
        float timer = 0f;
        
        while (timer <= duration)
        {
            HeadJoint.localRotation = Quaternion.Lerp(headStartValue, headEndValue, timer/duration);
            NeckJoint.localRotation = Quaternion.Lerp(neckStartValue, neckEndValue, timer/duration);
            timer += Time.deltaTime;
            yield return null;
        }
        
        // Make sure the final value is the right one
        HeadJoint.localRotation = headEndValue;
        NeckJoint.localRotation = neckEndValue;
    }


    #region Network

    // Used for User, sent by admin
    public void HeadPoseUpdateByValue(double poseRx, double poseRy, double poseRz)
    {
        double x = poseRx * Mathf.Rad2Deg, 
            y = poseRy * Mathf.Rad2Deg,
            z = poseRz * Mathf.Rad2Deg;

        Vector3 rot = new Vector3((float) x, (float) y, (float) z);
        
        HeadJoint.localRotation = CalculateQuaternionForPose(rot, _headMultiplier, HeadRotCorrection);
        NeckJoint.localRotation = CalculateQuaternionForPose(rot, _neckMultiplier, NeckRotCorrection);
    }

    // Used by Admin, will be sent to User
    private void MakeServerRequestForHeadPose(Dictionary<string, double> newPose)
    {
        if (MainManager.Instance.NetworkMode != NetworkMode.OFFLINE)
        {
            // Server request
            NetworkManager.Instance.AvatarHeadMoved(newPose["pose_Rx"], newPose["pose_Ry"], newPose["pose_Rz"]);
        }
    }

    private void SendHeadPoseTransitionToUser(Vector3 futureRot, float transitionDuration)
    {
        if (MainManager.Instance.NetworkMode != NetworkMode.OFFLINE)
            NetworkManager.Instance.AvatarPoseTransitionToNewFrame(JsonConvert.SerializeObject(futureRot), transitionDuration);
    }
    
    #endregion
    
}

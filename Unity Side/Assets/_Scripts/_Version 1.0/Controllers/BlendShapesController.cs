using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BlendShapesController : MonoBehaviour
{
    /// <summary>
    /// The skinned mesh renderer contains a parameter : blendshapes.
    /// Those blendshapes are used to make the shape key of the avatar move, on a scale from 0 to 100.
    /// Watch out : in Blender it's from 0.0 to 1.0, here it's 0.0 to 100.0
    /// You can access this value using SkinnedMeshRenderer.GetBlendShapeWeight(index of blendshape).
    /// You can edit this value using SkinnedMeshRenderer.SetBlendShapeWeight(index of blendshape, new float value of blendshape - from 1 to 100)
    /// </summary>
    [Header("Instances")] public SkinnedMeshRenderer SkinnedMeshRenderer;
    
    /// <summary>
    /// The dictionary is used because we send the name of the blendshapes that need to move from openface's csv.
    /// Then, to modify the value, we need the index of the blendshape
    /// This is why we gather both values in a dictionary
    /// </summary>
    private Dictionary<string, int> _avatarBlendShapesKeyAndIndexDict;

    // Tool
    private ConvertAUtoBlendShapes _convertAUtoBlendShapes;

    public void Init()
    {
        _convertAUtoBlendShapes = new ConvertAUtoBlendShapes();
        
        _avatarBlendShapesKeyAndIndexDict = new Dictionary<string, int>();
        for (int i = 0; i < SkinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
        {
            _avatarBlendShapesKeyAndIndexDict.Add(SkinnedMeshRenderer.sharedMesh.GetBlendShapeName(i), i);
        }
    }
    
    public void BlendShapeUpdateForFrame(Frame frame)
    {
         Dictionary<string, double> changesDict = _convertAUtoBlendShapes.GetDictOfBlendShapesChangedInFrame(frame);
         
         ChangeBlendShapesByDict(changesDict);

         SendBlendShapesMovementToUser(changesDict);
    }
    
    public void ChangeBlendShapesByDict(Dictionary<string, double> dict)
    {
        foreach (var change in dict)
        {
            ChangeAvatarBlendShapeValue(_avatarBlendShapesKeyAndIndexDict[change.Key], change.Value * 20); // change.Value * by 20 because unity's blend shapes are between 0 and 100 and not 0 and 5
        }
    }

    // this function is used to reset each blend shape smoothly
    public void MakeBlendShapesResetOverTime(float transitionDuration)
    {
        int blendShapeCount = SkinnedMeshRenderer.sharedMesh.blendShapeCount;
        
        // Will launch 82 coroutines
        for (int i = 0; i < blendShapeCount; i++)
        {
            StartCoroutine(MakeBlendShapeChangeTowardValueOverTime(SkinnedMeshRenderer.GetBlendShapeWeight(i), 0, i,
                transitionDuration));
        }
    }

    /// <summary>
    /// Transitions the avatar's blend shapes between the current frame and a target frame over a specified duration.
    /// </summary>
    /// <param name="frame">The target frame to transition to.</param>
    /// <param name="transitionDuration">The duration of the transition in seconds.</param>
    public void TransitionBetweenFrames(Frame frame, float transitionDuration)
    {
        Dictionary<string, double> futureChangesDict = _convertAUtoBlendShapes.GetDictOfBlendShapesChangedInFrame(frame);
        
        // Local
        TransitionToDict(futureChangesDict, transitionDuration);
        
        // Server
        SendBlendShapesTransitionToUser(futureChangesDict, transitionDuration);
    }

    public void TransitionToDict(Dictionary<string, double> futureChangesDict, float transitionDuration)
    {
        // Will launch 82 coroutines
        foreach (var futureChange in futureChangesDict)
        {
            StartCoroutine(
                MakeBlendShapeChangeTowardValueOverTime(
                    SkinnedMeshRenderer.GetBlendShapeWeight(_avatarBlendShapesKeyAndIndexDict[futureChange.Key]),
                    futureChange.Value * 20,
                    _avatarBlendShapesKeyAndIndexDict[futureChange.Key],
                    transitionDuration
                )
            );
        }

    }
    
    /// <summary>
    /// Coroutine that gradually changes the value of a blend shape over time.
    /// </summary>
    /// <param name="startValue">The initial value of the blend shape.</param>
    /// <param name="endValue">The target value the blend shape should reach.</param>
    /// <param name="blendShapeIndex">The index of the blend shape to change.</param>
    /// <param name="duration">The duration of the transition in seconds.</param>
    /// <returns>An IEnumerator used for the coroutine execution.</returns>
    private IEnumerator MakeBlendShapeChangeTowardValueOverTime(double startValue, double endValue, int blendShapeIndex, float duration)
    {
        float timer = 0f;
        while (timer <= duration)
        {
            ChangeAvatarBlendShapeValue(blendShapeIndex, 
                Mathf.Lerp(float.Parse(startValue.ToString()), float.Parse(endValue.ToString()), 
                timer/duration));
            timer += Time.deltaTime;
            yield return null;
        }
        
        // Make sure the final value is the right one
        ChangeAvatarBlendShapeValue(blendShapeIndex, endValue);
    }

    private void ChangeAvatarBlendShapeValue(int index, double value)
    {
        SkinnedMeshRenderer.SetBlendShapeWeight(index, float.Parse(value.ToString()));
    }

    #region Network

    private void SendBlendShapesMovementToUser(Dictionary<string, double> changesDict)
    {
        // Serialize changes and send them to server
        // It is useful because you do 1 request per tab instead of 1 per pair of string and double in the changesDict
        if (MainManager.Instance.NetworkMode != NetworkMode.OFFLINE)
            NetworkManager.Instance.AvatarBlendShapesMoved(JsonConvert.SerializeObject(changesDict));
    }

    private void SendBlendShapesTransitionToUser(Dictionary<string, double> changesDict, float transitionDuration)
    {
        if (MainManager.Instance.NetworkMode != NetworkMode.OFFLINE)
            NetworkManager.Instance.AvatarBlendShapeTransitionToNewFrame(JsonConvert.SerializeObject(changesDict), transitionDuration);
    }

    #endregion
}

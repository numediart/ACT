using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class FacsVersion_AutonomousAvatarInitialization : MonoBehaviour
{
    [FormerlySerializedAs("ZeroMqfacSvatarRef")] [Header("Main Script Ref")]
    public FacsVersion_ZeroMQFACSvatar facsVersionZeroMqfacSvatarRef;

    [Header("Avatars")]
    public GameObject AvatarToInit;
    public GameObject AvatarToReset;
    
    [Header("Keyword for Game Objects that need a script")]
    public string KeywordForArmature;
    public string KeywordForBody;
    
    [Header("Parameters for Initialization of scripts")]
    public string KeywordForNeck;
    public string KeywordForHead;
    public float Value1ForArmatureScript;
    public float Value2ForAmatureScript;

    // Intern var
    private GameObject _armatureGameObject;
    private FacsVersion_HeadRotatorBone _armatureScriptRef;
    private Transform _armatureHead;
    private Transform _armatureNeck;

    private Transform _bodyTransform;
    private FacsVersion_FACSnimator _bodyScriptRef;
    
    #region Listeners

    public void InitializeAvatar()
    {
        if (AvatarToInit == null)
            throw new Exception("Error : No avatar to initialize");
        
        AddArmatureScript();
        AddBodyScript();
        AddAvatarScript();
        InitializeMainScript();
    }

    public void ResetAvatar()
    {
        if (AvatarToReset == null)
            throw new Exception("Error : No avatar to reset");
        
        RemoveArmatureScript();
        RemoveBodyScript();
        RemoveAvatarScript();
    }

    #endregion
    
    #region Armature related functions

    private void AddArmatureScript()
    {
        foreach (Transform child in AvatarToInit.transform)
        {
            if (child.name.Contains(KeywordForArmature))
            {
                _armatureGameObject = child.gameObject;

                _armatureScriptRef = !_armatureGameObject.TryGetComponent<FacsVersion_HeadRotatorBone>(out var headRotatorBone) ? _armatureGameObject.AddComponent<FacsVersion_HeadRotatorBone>() : headRotatorBone;
                _armatureHead = FindTransformRecursivelyByKeyword(child, KeywordForHead);
                _armatureNeck = FindTransformRecursivelyByKeyword(child, KeywordForNeck);
                
                //Initialize script
                _armatureScriptRef.jointObj_head = _armatureHead;
                _armatureScriptRef.jointObj_neck = _armatureNeck;
                _armatureScriptRef.headRotCorrection = Value1ForArmatureScript;
                _armatureScriptRef.neckRotCorrection = Value2ForAmatureScript;
                
                break;
            }
        }
    }

    private void RemoveArmatureScript()
    {
        foreach (Transform child in AvatarToReset.transform)
        {
            if (child.name.Contains(KeywordForArmature) && _armatureGameObject.TryGetComponent<FacsVersion_HeadRotatorBone>(out var headRotatorBone))
            {
                DestroyImmediate(headRotatorBone);

                break;
            }
        }
    }
    
    #endregion

    #region Body related functions

    private void AddBodyScript()
    {
        foreach (Transform child in AvatarToInit.transform)
        {
            if (child.name.Contains(KeywordForBody) && !child.gameObject.TryGetComponent<FacsVersion_FACSnimator>(out var facs))
            {
                _bodyTransform = child;
                _bodyScriptRef = _bodyTransform.gameObject.AddComponent<FacsVersion_FACSnimator>();
                
                break;
            }
        }
    }

    private void RemoveBodyScript()
    {
        foreach (Transform child in AvatarToReset.transform)
        {
            if (child.name.Contains(KeywordForBody) && child.gameObject.TryGetComponent<FacsVersion_FACSnimator>(out var facs))
            {
                DestroyImmediate(facs);
                
                break;
            }
        }
    }

    #endregion

    #region Avatar related functions

    private void AddAvatarScript()
    {
        if (AvatarToInit.TryGetComponent<FacsVersion_HeadCullingFPS>(out var headCullingFPS))
            return;
        
        AvatarToInit.AddComponent<FacsVersion_HeadCullingFPS>();
    }

    private void RemoveAvatarScript()
    {
        if (AvatarToReset.TryGetComponent<FacsVersion_HeadCullingFPS>(out var headCullingFPS))
            DestroyImmediate(headCullingFPS);
    }

    #endregion

    #region MainScript related functions

    private void InitializeMainScript()
    {
        facsVersionZeroMqfacSvatarRef.participants = Participants.users_1_models_1;
        facsVersionZeroMqfacSvatarRef.FACSModel0 = _bodyScriptRef;
        facsVersionZeroMqfacSvatarRef.RiggedModel0 = _armatureScriptRef;
        
        facsVersionZeroMqfacSvatarRef.FACSModel1 = null;
        facsVersionZeroMqfacSvatarRef.RiggedModel1 = null;
        facsVersionZeroMqfacSvatarRef.FACSModelDnn = null;
        facsVersionZeroMqfacSvatarRef.RiggedModelDnn = null;
    }

    #endregion
    
    private Transform FindTransformRecursivelyByKeyword(Transform originalTransformToLookInto, string keyword)
    {
        // Iterate through all children and find the GameObject with the specified name
        foreach (Transform child in originalTransformToLookInto)
        {
            if (child.name.Contains(keyword))
            {
                return child;
            }
            
            // Perform recursive search on the child's children
            Transform foundObject = FindTransformRecursivelyByKeyword(child, keyword);
            
            // If the target GameObject is found in the child's children, return it
            if (foundObject != null)
            {
                return foundObject;
            }
        }
        
        return null;
    }
}

using System;
using UnityEngine;
using UnityEngine.Serialization;

public class NewAvatarInitializer : MonoBehaviour
{
    [Header("Main Manager")] [SerializeField] private MainManager _mainManagerRef;

    [Header("Avatar")] public GameObject NewAvatar;

    [Header("Keywords")] public string BlendShapesGameObject;
    public string NeckJoint;
    public string HeadJoint;

    [Header("Values")] public Vector3 HeadRotCorrection;
    public Vector3 NeckRotCorrection;

    private Transform _jointHead;
    private Transform _jointNeck;
    private Transform _bodyTransform;

    #region Listeners

    public void InitNewAvatar()
    {
        if (NewAvatar == null)
            throw new Exception("Error : No avatar to initialize");
        
        AddSkinnedMeshRendererRefToBlendShapesController();
        AddReferencesToHeadPoseController();
    }

    public void ResetNewAvatar()
    {
        if (NewAvatar == null)
            throw new Exception("Error : No avatar to reset");
    }

    public void Test()
    {
        if (NewAvatar == null)
            throw new Exception("Error : No avatar to test");
    }

    #endregion

    private void AddSkinnedMeshRendererRefToBlendShapesController()
    {
        foreach (Transform child in NewAvatar.transform)
        {
            if (child.name.Contains(BlendShapesGameObject))
            {
                _bodyTransform = child;

                _mainManagerRef.BlendShapesController.SkinnedMeshRenderer =
                    _bodyTransform.GetComponent<SkinnedMeshRenderer>();
                
                break;
            }
        }
    }

    private void AddReferencesToHeadPoseController()
    {
        foreach (Transform child in NewAvatar.transform)
        {
            _jointNeck = FindTransformRecursivelyByKeyword(child, NeckJoint);
            _jointHead = FindTransformRecursivelyByKeyword(child, HeadJoint);

            if (_jointHead != null && _jointNeck != null)
            {
                _mainManagerRef.HeadPoseController.HeadJoint = _jointHead;
                _mainManagerRef.HeadPoseController.NeckJoint = _jointNeck;
                
                _mainManagerRef.HeadPoseController.HeadRotCorrection = HeadRotCorrection;
                _mainManagerRef.HeadPoseController.NeckRotCorrection = NeckRotCorrection;
                break;
            }
        }
    }
    
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

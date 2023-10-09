using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class AlphaVer_AutonomousLandmarkMaker : MonoBehaviour
{
    [FormerlySerializedAs("MainScriptRef")] [Header("Instances")]
    public AlphaVer_MainScript alphaVerMainScriptRef;
    public Transform AvatarContainerTransform;
    [FormerlySerializedAs("ManualEventList")] public AlphaVer_ManualEventList alphaVerManualEventList;
    public string KeywordForLandmarkContainer;
    public List<string> KeywordsForLandmarks;

    [Header("Verification")]
    public Transform AvatarTransform;
    public Transform LandmarkContainerTransform;

    #region Initiation Scripts
    private void InitiateTransforms()
    {
        if (AvatarContainerTransform.childCount <= 0)
            throw new Exception("There is no Avatar in the Avatars Container");
        
        AvatarTransform = AvatarContainerTransform.GetChild(0);

        for (int i = 0; i < AvatarTransform.childCount; i++)
        {
            if (AvatarTransform.GetChild(i).name.Contains(KeywordForLandmarkContainer))
            {
                LandmarkContainerTransform = AvatarTransform.GetChild(i);
                break;
            }
        }

        if (LandmarkContainerTransform == null)
            throw new Exception("LandmarkContainerTransform not found");    
    }

    private void AddControllerScripts()
    {
        AlphaVer_AvatarController alphaVerAvatarController = AvatarTransform.GetComponent<AlphaVer_AvatarController>();
        AlphaVer_HeadController alphaVerHeadController = AvatarTransform.GetComponent<AlphaVer_HeadController>();
        
        // Add
        if (!alphaVerAvatarController)
        {
            alphaVerAvatarController = AvatarTransform.AddComponent<AlphaVer_AvatarController>();
        }

        if (!alphaVerHeadController)
        {
            alphaVerHeadController = AvatarTransform.AddComponent<AlphaVer_HeadController>();
        }

        // Initiate
        alphaVerAvatarController.alphaVerHeadController = alphaVerHeadController;
        alphaVerAvatarController.ManualEvents = alphaVerManualEventList.ManualEvents;

        alphaVerMainScriptRef.alphaVerAvatarController = alphaVerAvatarController;
    }

    #endregion
    
    #region Recursive functions
    
    private void AddLandmarksToGameObjectsRecursively(Transform parent)
    {
        // Loop through keywords to see if parent's name contains one of them
        foreach (var keyword in KeywordsForLandmarks)
        {
            // if no landmark applied already
            if (parent.name.Contains(keyword) && !parent.gameObject.GetComponent<AlphaVer_Landmark>())
            {
                // Save landmark for main variable
                AlphaVer_Landmark alphaVerLandmark = parent.AddComponent<AlphaVer_Landmark>();
                alphaVerMainScriptRef.alphaVerAvatarController.alphaVerHeadController.Landmarks.Add(alphaVerLandmark);
                // break the loop when added Landmark
                break;
            }
        }

        // Recursively loop through children
        for (int i = 0; i < parent.childCount; i++)
        {
            AddLandmarksToGameObjectsRecursively(parent.GetChild(i));
        }
    }

    private void RemoveLandmarksFromGameObjectsRecursively(Transform parent)
    {
        // See if parent has landmark
        if (parent.TryGetComponent<AlphaVer_Landmark>(out var component))
        {
            // Remove the component
            DestroyImmediate(component);
        }

        // Recursively loop through children
        for (int i = 0; i < parent.childCount; i++)
        {
            RemoveLandmarksFromGameObjectsRecursively(parent.GetChild(i));
        }
    }
    
    #endregion

    #region Listeners on buttons
    
    public void LaunchLandmarkMaker()
    {
        InitiateTransforms();
        
        AddControllerScripts();
        
        AddLandmarksToGameObjectsRecursively(LandmarkContainerTransform);
    }
    
    public void ResetLandmarkMaker()
    {
        // Landmarks
        RemoveLandmarksFromGameObjectsRecursively(LandmarkContainerTransform);
        alphaVerMainScriptRef.alphaVerAvatarController.alphaVerHeadController.Landmarks = new List<AlphaVer_Landmark>();

        // Controller scripts
        if (AvatarTransform.TryGetComponent<AlphaVer_AvatarController>(out var avatarController))
        {
            DestroyImmediate(avatarController);
        }
        if (AvatarTransform.TryGetComponent<AlphaVer_HeadController>(out var headController))
        {
            DestroyImmediate(headController);
        }
        
        // Transforms
        AvatarTransform = null;
        LandmarkContainerTransform = null;
    }

    public void LoadLandMarksToJson()
    {
        alphaVerMainScriptRef.betaVerJsonWritter.OutputJson();
    }

    public void GetAllLandmarksWorldCoordinates()
    {
        alphaVerMainScriptRef.betaVerJsonWritter.MyGoWcList = new BetaVer_JsonWritter.GoWcList();
        
        foreach (var landmark in alphaVerMainScriptRef.alphaVerAvatarController.alphaVerHeadController.Landmarks)
        {
            alphaVerMainScriptRef.betaVerJsonWritter.FillGoWcList(landmark);
        }
    }
    
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionIntensityUI : MonoBehaviour
{
    [SerializeField] private int _intensityId;
    
    public void ActionIntensityClicked()
    {
        MainManager.Instance.AvatarActionIntensitySelected(_intensityId);
    }
}

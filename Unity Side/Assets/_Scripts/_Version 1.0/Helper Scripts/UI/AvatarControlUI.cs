using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class AvatarControlUI : MonoBehaviour
{
    [Header("Instance")] [SerializeField] private TextMeshProUGUI _actionName;
    [HideInInspector] public int NbIntensities;
    [HideInInspector] public int ActionIndex;

    public void ModifyActionName(string newName)
    {
        _actionName.text = newName;
    }
    
    #region Listeners

    public void ActionControlClicked()
    {
        MainManager.Instance.AvatarActionSelected(ActionIndex, NbIntensities);
    }   
    
    #endregion
}

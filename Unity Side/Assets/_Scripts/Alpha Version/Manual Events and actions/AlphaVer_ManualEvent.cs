using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AlphaVer_ManualEvent : MonoBehaviour
{
    [Header("Infos")] public string EventName;
    [SerializeField] private AudioClip _audioClip;
    
    //[Header("Parameters")]
    [SerializeField] private List<AlphaVer_ManualAction> _actions;

    public void ExecuteEvent()
    {
        if (AlphaVer_MainScript.s_instance.alphaVerMode == AlphaVer_Mode.MANUAL)
        {
            AlphaVer_MainScript.s_instance.alphaVerAudioManager.PlayAudioClip(_audioClip);
            foreach (var action in _actions)
            {
                action.ExecuteAction();
            }
        }
    }
}

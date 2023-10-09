using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlphaVer_ManualEventUI : MonoBehaviour
{
    [Header("Instances")] [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _buttonText;

    public void AddListenerAndChangeTextFromManualEvent(AlphaVer_ManualEvent alphaVerManualEvent)
    {
        _button.onClick.AddListener(alphaVerManualEvent.ExecuteEvent);
        _buttonText.text = alphaVerManualEvent.EventName;
    }
}

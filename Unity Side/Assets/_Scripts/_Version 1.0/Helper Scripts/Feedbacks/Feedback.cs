using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Feedback : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _message;
    public FeedbackType FeedbackType;

    public void ModifyMessage(string message)
    {
        _message.text = message;
    }

    public void StartFeedBackLifeCycle()
    {
        
    }
}

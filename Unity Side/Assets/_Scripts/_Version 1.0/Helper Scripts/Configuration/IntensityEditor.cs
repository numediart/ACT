using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntensityEditor : MonoBehaviour
{
    [Header("Instances")] [SerializeField] private TMP_InputField _csvInput;
    [SerializeField] private TMP_InputField _audioInput;

    public string CsvInput => _csvInput.text;
    public string AudioInput => _audioInput.text;
    
    //intern var
    private string _newCsvInput;
    private string _newAudioInput;
    
    #region Listeners

    public void OnCsvInputChange(string newStr)
    {
        _newCsvInput = newStr;
    }

    public void OnAudioInputChange(string newStr)
    {
        _newAudioInput = newStr;
    }
    
    #endregion

    public void EmptyInputs()
    {
        _csvInput.text = "";
        _audioInput.text = "";
    }

    public void FillInputWithAvatarAction(AvatarAction avatarAction)
    {
        _csvInput.text = avatarAction.CsvFilePath;
        _audioInput.text = avatarAction.AudioFilePath;
    }
    
    #region Getters


    
    #endregion
}

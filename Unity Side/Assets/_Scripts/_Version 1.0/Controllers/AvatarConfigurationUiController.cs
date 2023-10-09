using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvatarConfigurationUiController : MonoBehaviour
{
    [Header("Instances")] [SerializeField] private Toggle _fullExpressionToggler;
    [SerializeField] private TMP_InputField _transitionDurationInput;
    /// <summary>
    /// Watch out for the order of the input fields : X if [0], Y is [1], Z is [2]
    /// </summary>
    [SerializeField] private TMP_InputField[] _headRotCorrectionInputs;
    [SerializeField] private TMP_InputField[] _neckRotCorrectionInputs;
    
    // Intern var
    private AvatarConfiguration _avatarConfigurationRef;
    private List<TMP_InputField> _allNumbersInputs;

    private void EmptyInputs()
    {
        _transitionDurationInput.text = "";

        if (_headRotCorrectionInputs.Length != _neckRotCorrectionInputs.Length)
        {
            throw new Exception("Not the same number of inputs for head and neck rot correction");
        }
        
        for (int i = 0; i < _headRotCorrectionInputs.Length; i++)
        {
            _headRotCorrectionInputs[i].text = "";
            _neckRotCorrectionInputs[i].text = "";
        }
    }

    public void Init(AvatarConfiguration acToInit)
    {
        _avatarConfigurationRef = acToInit;

        _fullExpressionToggler.isOn = acToInit.FullExpressionOn;
        
        _transitionDurationInput.text = _avatarConfigurationRef.TransitionDelay.ToString();

        _headRotCorrectionInputs[0].text = _avatarConfigurationRef.HeadRotCorrection.x.ToString();
        _headRotCorrectionInputs[1].text = _avatarConfigurationRef.HeadRotCorrection.y.ToString();
        _headRotCorrectionInputs[2].text = _avatarConfigurationRef.HeadRotCorrection.z.ToString();

        _neckRotCorrectionInputs[0].text = _avatarConfigurationRef.NeckRotCorrection.x.ToString();
        _neckRotCorrectionInputs[1].text = _avatarConfigurationRef.NeckRotCorrection.y.ToString();
        _neckRotCorrectionInputs[2].text = _avatarConfigurationRef.NeckRotCorrection.z.ToString();

        _allNumbersInputs = new List<TMP_InputField>();
        _allNumbersInputs.Add(_transitionDurationInput);
        
        if (_headRotCorrectionInputs.Length != _neckRotCorrectionInputs.Length)
        {
            throw new Exception("Not the same number of inputs for head and neck rot correction");
        }
        
        for (int i = 0; i < _headRotCorrectionInputs.Length; i++)
        {
            _allNumbersInputs.Add(_headRotCorrectionInputs[i]);
            _allNumbersInputs.Add(_neckRotCorrectionInputs[i]);
        }
    }

    public AvatarConfiguration ConfirmChangesEvent()
    {
        if (InputsEmptyOrWronglyFilled())
            return null;
        
        _avatarConfigurationRef.TransitionDelay = float.Parse(_transitionDurationInput.text);
        
        _avatarConfigurationRef.HeadRotCorrection = new Vector3(float.Parse(_headRotCorrectionInputs[0].text),
            float.Parse(_headRotCorrectionInputs[1].text),
            float.Parse(_headRotCorrectionInputs[2].text));
        
        _avatarConfigurationRef.NeckRotCorrection = new Vector3(float.Parse(_neckRotCorrectionInputs[0].text),
            float.Parse(_neckRotCorrectionInputs[1].text),
            float.Parse(_neckRotCorrectionInputs[2].text));

        _avatarConfigurationRef.FullExpressionOn = _fullExpressionToggler.isOn;

        return _avatarConfigurationRef;
    }

    private bool InputsEmptyOrWronglyFilled()
    {
        foreach (var nbInput in _allNumbersInputs)
        {
            if (string.IsNullOrEmpty(nbInput.text))
                return true;
        }

        return false;
    }
}

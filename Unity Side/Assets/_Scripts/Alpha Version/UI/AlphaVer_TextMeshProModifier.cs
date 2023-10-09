using System;
using System.Text;
using TMPro;
using UnityEngine;

[System.Serializable]
public class AlphaVer_TextMeshProModifier
{
    [Header("Instance")] [SerializeField] private TextMeshProUGUI _textMeshProToModify;
    
    [Header("Parameters")]
    [SerializeField] private string _prefix;
    [SerializeField] private string _suffix;
    [SerializeField] private Color _textColor;

    public void ModifyText(string newStr)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(_prefix.Trim());
        sb.Append(" ");
        sb.Append(newStr.Trim());
        sb.Append(" ");
        sb.Append(_suffix.Trim());

        _textMeshProToModify.text = sb.ToString();
    }

    public void ColorText()
    {
        _textMeshProToModify.color = _textColor;
    }
}

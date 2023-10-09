using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConfigurationAction : MonoBehaviour
{
    [Header("Instances")] [SerializeField] private TextMeshProUGUI _displayedName;

    public void ChangeDisplayedName(string displayedName)
    {
        _displayedName.text = displayedName;
    }

    public void ConfigurationActionSelected()
    {
        ConfigurationManager.Instance.ActionConfigurationController.ConfigurationActionSelectedEvent(_displayedName.text);
    }
}

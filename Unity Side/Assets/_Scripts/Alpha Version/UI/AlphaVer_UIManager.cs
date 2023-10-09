using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AlphaVer_UIManager : MonoBehaviour
{
    [Header("Infos")] [SerializeField] private AlphaVer_TextMeshProModifier _status;

    [Header("HUDs")] [SerializeField] private GameObject _hudButton;
    [FormerlySerializedAs("_hudManual")] [SerializeField] private AlphaVer_ManualHud hudAlphaVerManual;

    [Header("Options")] [SerializeField] private KeyCode _toggleHudButtonVisibility = KeyCode.H;
    
    // Start is called before the first frame update
    void Start()
    {
        _status.ColorText();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(_toggleHudButtonVisibility))
        {
            ToggleHudButtonVisibility();
        }
    }

    #region HUD Button

    private void ToggleHudButtonVisibility()
    {
        _hudButton.SetActive(!_hudButton.activeSelf);
    }

    #endregion

    #region HUD Manual

    public void HideHudManual()
    {
        hudAlphaVerManual.gameObject.SetActive(false);
    }

    public void ShowHudManual()
    {
        hudAlphaVerManual.gameObject.SetActive(true);
    }

    public void InitiateHudManual(List<AlphaVer_ManualEvent> manualEvents)
    {
        hudAlphaVerManual.Initiate(manualEvents);
    }

    #endregion

    #region Status

    public void ChangeStatus(string text)
    {
        _status.ModifyText(text);
    }

    #endregion
}

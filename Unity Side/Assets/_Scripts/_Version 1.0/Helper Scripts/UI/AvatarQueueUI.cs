using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AvatarQueueUI : MonoBehaviour
{
    [Header("Instance")] public TextMeshProUGUI ActionName;
    public TextMeshProUGUI ActionIntensity;
    public TextMeshProUGUI ActionNb;
    public Image BackgroundImage;

    //Intern var
    [SerializeField] private int _id;
    
    public void ModifyActionName(string newName)
    {
        ActionName.text = newName;
    }

    public void ModifyBackgroundColor(Color newColor)
    {
        BackgroundImage.color = newColor;
    }

    public void ModifyActionIntensity(string intensity)
    {
        ActionIntensity.text = intensity;
    }

    public void ModifyActionNb(int nb)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("N° ");
        sb.Append(nb);
        ActionNb.text = sb.ToString();
    }

    public void SetActionIndex(int index)
    {
        _id = index;
    }

    public void DeleteActionFromQueue()
    {
        MainManager.Instance.ActionDeletedFromQueueEvent(_id);
    }
}

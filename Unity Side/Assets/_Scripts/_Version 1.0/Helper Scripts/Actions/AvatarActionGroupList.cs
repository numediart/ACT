using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AvatarActionGroupList : MonoBehaviour
{
    public List<AvatarActionGroup> AvatarActionGroups = new List<AvatarActionGroup>();

    public void Init()
    {
        AvatarActionGroups = MainManager.Instance.JsonReader.CreateAvatarActionGroupByConfigFile();

        foreach (var actionGroup in AvatarActionGroups)
        {
            actionGroup.Init();
        }
        
        MainManager.Instance.ActionListCreationFinishedEvent();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AvatarActionGroup
{
    public string Name;
    public List<AvatarAction> AvatarActions = new List<AvatarAction>();
    
    public void Init()
    {
        foreach (var action in AvatarActions)
        {
            action.Init();
        }
    }
}

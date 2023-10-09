using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class AvatarActionQueue
{
    public int CurrentActionIndex = 0;
    [SerializeField] private List<AvatarAction> _queue = new List<AvatarAction>();

    public AvatarAction GetCurrentAvatarAction()
    {
        return _queue.Count - 1 < CurrentActionIndex ? null : _queue[CurrentActionIndex];
    }

    public void CurrentAvatarActionFinished()
    {
        CurrentActionIndex++;
    }

    public void AddActionToQueue(AvatarAction action)
    {
        _queue.Add(action);
    }

    public void CurrentAvatarActionSkipped()
    {
        CurrentActionIndex++;
    }

    public void CurrentAvatarActionBacked()
    {
        if (CurrentActionIndex > 0)
            CurrentActionIndex--;
    }
    
    public void RemoveActionFromQueueByIndex(int actionIndex)
    {
        if (actionIndex <= CurrentActionIndex && CurrentActionIndex > 0)
            CurrentActionIndex--;
        
        _queue.RemoveAt(actionIndex);
    }

    public bool HasOtherActionAfterCurrent()
    {
        return _queue.Count - 1 >= CurrentActionIndex + 1;
    }
}

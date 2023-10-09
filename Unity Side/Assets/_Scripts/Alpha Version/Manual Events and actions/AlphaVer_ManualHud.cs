using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AlphaVer_ManualHud : MonoBehaviour
{
    [Header("Instances")] [SerializeField] private GameObject _info;
    [SerializeField] private GameObject _scrollView;
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject _manualEventPrefab;
    
    // intern var
    private List<GameObject> _manualEventsGameObjects;

    public void Initiate(List<AlphaVer_ManualEvent> manualEvents)
    {
        if (manualEvents.Count == 0)
        {
            DisplayInfo();
        }
        else if (manualEvents.Count >= 1)
        {
            DisplayScrollView();
            FillScrollView(manualEvents);
        }
        else
        {
            throw new Exception("manualEvents.count < 0");
        }
    }

    private void DisplayInfo()
    {
        _scrollView.SetActive(false);
        _info.SetActive(true);
    }

    private void DisplayScrollView()
    {
        _scrollView.SetActive(true);
        _info.SetActive(false);
    }

    private void FillScrollView(List<AlphaVer_ManualEvent>manualEvents)
    {
        _manualEventsGameObjects = new List<GameObject>();
        
        foreach (var manualEvent in manualEvents)
        {
            _manualEventsGameObjects.Add(Instantiate(_manualEventPrefab, _content.transform));
            _manualEventsGameObjects[^1].GetComponent<AlphaVer_ManualEventUI>().AddListenerAndChangeTextFromManualEvent(manualEvent);
        }
    }
}

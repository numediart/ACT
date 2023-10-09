using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [Header("Instances")] [SerializeField] private AudioSource _audioSource;
    
    // intern var
    private float _startTime;

    public void Init()
    {
        if (_audioSource == null)
            _audioSource.GetComponent<AudioSource>();

        _startTime = MainManager.Instance.TransitionDuration;
    }

    //TODO: modify start Time depending on the use of full expression mode or not
    public void PlayAudioClip(AudioClip audioClip)
    {
        if (_audioSource.isPlaying)
            _audioSource.Stop();
        
        _audioSource.clip = audioClip;
        _audioSource.time = _startTime;

        _audioSource.Play();
    }

    public void PauseAudioClip()
    {
        if (_audioSource.isPlaying)
            _audioSource.Pause();
    }

    public void ResumeAudioClip()
    {
        if (_audioSource.clip != null)
            _audioSource.Play();
    }

    public void StopAudioClip()
    {
        if (_audioSource.isPlaying)
            _audioSource.Stop();
    }
}


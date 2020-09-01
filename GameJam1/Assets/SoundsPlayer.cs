using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip _collectGoldSound; 
    [SerializeField] private AudioClip _crashSound;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayGoldSound()
    {
        _audioSource.clip = _collectGoldSound;
        _audioSource.Play();
    }
    
    public void PlayCrashSound()
    {
        _audioSource.clip = _crashSound;
        _audioSource.Play();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    private StoveCounter _stoveCounter;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _stoveCounter = GetComponentInParent<StoveCounter>();
    }

    private void Start()
    {
        _stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool playSound = e.State == StoveCounter.StoveCounterState.Frying || e.State == StoveCounter.StoveCounterState.Fried;
        if (playSound)
        {
            _audioSource.Play();
        }
        else
        {
            _audioSource.Pause();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    private StoveCounter _stoveCounter;
    private AudioSource _audioSource;
    private float _warningSoundTimer;
    private float _warningSoundTimerMax = 0.2f;
    private bool _playWarningSound;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _stoveCounter = GetComponentInParent<StoveCounter>();
    }

    private void Start()
    {
        _stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
        _stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
    }

    private void Update()
    {
        if(!_playWarningSound)
        {
            return;
        }
        _warningSoundTimer -= Time.deltaTime;
        if(_warningSoundTimer <= 0)
        {
            _warningSoundTimer = _warningSoundTimerMax;
            SoundManager.Instance.PlayWarningSound(transform.position);
        }
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
    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnShowProgressAmount = 0.5f;
        _playWarningSound = _stoveCounter.GetStoveCounterState() == StoveCounter.StoveCounterState.Fried && e.progressNormalized >= burnShowProgressAmount;
    }
}

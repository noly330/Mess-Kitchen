using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVistual : MonoBehaviour
{
    [SerializeField] private StoveCounter _stoveCounter;
    [SerializeField] private GameObject _stoveOnGameObejct;
    [SerializeField] private GameObject _particlesGameobject;

    
    private void Start()
    {
        _stoveCounter.OnStateChanged += OnStateChanged;
    }

    private void OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool showVisual = e.State != StoveCounter.StoveCounterState.Idle;
        _stoveOnGameObejct.SetActive(showVisual);
        _particlesGameobject.SetActive(showVisual);
    }
}

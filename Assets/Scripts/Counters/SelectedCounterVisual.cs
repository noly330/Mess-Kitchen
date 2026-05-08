using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    private BaseCounter _baseCounter;
    [SerializeField] private GameObject[] _selectedCounterVisuals;

    private void Awake()
    {
        _baseCounter = GetComponentInParent<BaseCounter>();
    }
    private void Start()
    {
        if (Player.LocalInstance != null)
        {

            Player.LocalInstance.OnSelectedCounterChanged += OnSelectedCounterChanged;
        }
        else
        {
            Player.OnAnyPlayerSpawned += OnSelectedCounterChanged;
        }
    }

    private void OnSelectedCounterChanged(object sender, EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChanged -= OnSelectedCounterChanged;
            Player.LocalInstance.OnSelectedCounterChanged += OnSelectedCounterChanged;
        }
    }

    private void OnSelectedCounterChanged(object sender, Player.SelectedCounterChangedEventArgs e)
    {
        if (_baseCounter == e.selectedCounter)
        {
            ShowSelectedCounterVisual();
        }
        else
        {
            HideSelectedCounterVisual();
        }
    }

    private void ShowSelectedCounterVisual()
    {
        foreach (GameObject item in _selectedCounterVisuals)
        {
            item.SetActive(true);
        }
    }

    private void HideSelectedCounterVisual()
    {
        foreach (GameObject item in _selectedCounterVisuals)
        {
            item.SetActive(false);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBurnWarningUI : MonoBehaviour
{
    private StoveCounter _stoveCounter;

    private void Awake() {
        _stoveCounter = GetComponentInParent<StoveCounter>();
    }

    private void Start()
    {
        _stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
        Hide();
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnShowProgressAmount = 0.5f;
        bool show = e.progressNormalized >= burnShowProgressAmount && _stoveCounter.GetStoveCounterState() == StoveCounter.StoveCounterState.Fried;

        if(show)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}

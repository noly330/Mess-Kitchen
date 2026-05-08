using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBurnFlashBarUI : MonoBehaviour
{
    private StoveCounter _stoveCounter;

    private Animator _animator;

    private void Awake() {
        _stoveCounter = GetComponentInParent<StoveCounter>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnShowProgressAmount = 0.5f;
        bool show = e.progressNormalized >= burnShowProgressAmount && _stoveCounter.GetStoveCounterState() == StoveCounter.StoveCounterState.Fried;

        _animator.SetBool(AnimatorHash.IsFlashing, show);
    }

 
}

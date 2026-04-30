using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{
    [SerializeField] private CuttingCounter _cuttingCounter;
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _cuttingCounter.OnCut += OnCut;
    }

    private void OnCut(object sender, EventArgs e)
    {
        _animator.SetTrigger(AnimatorHash.Cut);
    }
}

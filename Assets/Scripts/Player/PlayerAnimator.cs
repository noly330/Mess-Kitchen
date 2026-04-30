using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Player _player;
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        _animator.SetBool(AnimatorHash.IsWalking, _player.IsWalking);
    }
}

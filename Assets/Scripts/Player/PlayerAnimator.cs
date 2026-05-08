using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    private Player _player;
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        if(!IsOwner) return;
        _animator.SetBool(AnimatorHash.IsWalking, _player.IsWalking);
    }
}

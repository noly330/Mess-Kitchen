using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private Player _player;

    private float footstepTimer;
    private float footstepTimerMax = 0.25f;
    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        footstepTimer -= Time.deltaTime;
        if(footstepTimer < 0f)
        {
            footstepTimer = footstepTimerMax;

            if(_player.IsWalking)
            {
                float volume = 1f;
                SoundManager.Instance.PlayFootstepsSound(transform.position,volume);
            }
        }
    }
}

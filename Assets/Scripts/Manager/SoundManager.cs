using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance{get;private set;}
    [SerializeField] private AudioClipRefsSO _audioClipRefsSO;

    private void Awake()
    {

        Instance = this;
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.Instance.OnPickedSomething += Player_OnPickedSomething;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObejctTrashed += TrashCounter_OnAnyObejctTrashed;
    }

    public void PlayFootstepsSound(Vector3 position, float volume)
    {
        PlaySound(_audioClipRefsSO.footstep, position, volume);
    }

    private void TrashCounter_OnAnyObejctTrashed(object sender, EventArgs e)
    {
        TrashCounter trashCounter = (sender as TrashCounter);
        PlaySound(_audioClipRefsSO.trash,trashCounter.transform.position);
    }

    private void BaseCounter_OnAnyObjectPlacedHere(object sender, EventArgs e)
    {
        BaseCounter baseCounter = (sender as BaseCounter);
        PlaySound(_audioClipRefsSO.objectDrop,baseCounter.transform.position);
    }

    private void Player_OnPickedSomething(object sender, EventArgs e)
    {
        PlaySound(_audioClipRefsSO.objectPickUp,Player.Instance.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, EventArgs e)
    {
        CuttingCounter cuttingCounter = (sender as CuttingCounter);
        PlaySound(_audioClipRefsSO.chop,cuttingCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, EventArgs e)
    {
        PlaySound(_audioClipRefsSO.deliverySuccess,transform.position);
    }
    private void DeliveryManager_OnRecipeFailed(object sender, EventArgs e)
    {
        PlaySound(_audioClipRefsSO.deliveryFailed,transform.position);
    }


    private void PlaySound(AudioClip[] clips,Vector3 position,float volume = 1f)
    {
        AudioClip clip = clips[UnityEngine.Random.Range(0,clips.Length)];
        PlaySound(clip,position,volume);
    }

    private void PlaySound(AudioClip clip,Vector3 position,float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(clip,position,volume);
    }

    
}

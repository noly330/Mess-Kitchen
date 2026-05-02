using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/AudioClipRefsSO")]
public class AudioClipRefsSO : ScriptableObject
{
   public AudioClip[] chop;
   public AudioClip[] deliveryFailed;
   public AudioClip[] deliverySuccess;
   public AudioClip[] footstep;
   public AudioClip[] objectDrop;
   public AudioClip[] objectPickUp;
   public AudioClip[] stoveSizzle;
   public AudioClip[] trash;
   public AudioClip[] warning;
}

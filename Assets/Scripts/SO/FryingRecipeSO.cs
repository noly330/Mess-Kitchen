using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/FryingRecipeSO")]
public class FryingRecipeSO : ScriptableObject
{
    public KitchenObjectSO inputKitchenObjectSO;
    public KitchenObjectSO outputKitchenObjectSO;
    public float fryingProgressMax;
}

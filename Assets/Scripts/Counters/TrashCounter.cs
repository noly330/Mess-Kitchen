using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObejctTrashed;
    new public static void ResetStaticData()
    {
        OnAnyObejctTrashed = null;
    }
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            player.GetKitchenObject().DestroySelf();
            OnAnyObejctTrashed?.Invoke(this, EventArgs.Empty);
        }
    }

}

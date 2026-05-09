using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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
            KitchenObject.DestoryKitchenObject(player.GetKitchenObject());
            InteractLogicServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        OnAnyObejctTrashed?.Invoke(this, EventArgs.Empty);
    }
}

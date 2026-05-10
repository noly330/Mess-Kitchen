using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    // public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    // public class OnProgressChangedEventArgs : EventArgs
    // {
    //     public float progressNormalized;
    // }
    public static event EventHandler OnAnyCut;
    public event EventHandler OnCut;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    [SerializeField] private CuttingRecipeSO[] _cuttingRecipeSO;
    private int _cuttingProgress;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject() && HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
            {
                //先缓存一下放置到柜台上的厨房物品，
                KitchenObject kitchenObject = player.GetKitchenObject();
                kitchenObject.SetKitchenObjectParent(this);  //客户端的话是有延迟的，但是已经缓存了没关系
                InteractLogicPlaceObjectOnCounterServerRpc();

            }
            else
            {
                //TODO: 玩家没拿任何东西
            }
        }
        else  //如果柜台上有东西
        {
            //如果玩家手上有东西
            if (player.HasKitchenObject())
            {
                //如果玩家手上的东西是盘子
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestoryKitchenObject(GetKitchenObject());
                    }
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc()
    {
        InteractLogicPlaceObjectOnCounterClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlaceObjectOnCounterClientRpc()
    {
        _cuttingProgress = 0;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() { progressNormalized = _cuttingProgress });
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            CutObjectServerRpc();
            TestCuttingProgressDoneServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        CutObjectClientRpc();
    }
    [ClientRpc]
    private void CutObjectClientRpc()
    {
        _cuttingProgress++;
        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() { progressNormalized = ((float)_cuttingProgress) / cuttingRecipeSO.cuttingProgressMax });

    }

    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRpc()
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
        if (_cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
        {

            KitchenObjectSO outputKitchenObjectSO = GetOutPutForInput(GetKitchenObject().GetKitchenObjectSO());

            KitchenObject.DestoryKitchenObject(GetKitchenObject());
            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);

        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        return GetCuttingRecipeSOWithInput(inputKitchenObjectSO) != null;
    }

    private KitchenObjectSO GetOutPutForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.outputKitchenObjectSO;
        }
        return null;
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (var cuttingRecipeSO in _cuttingRecipeSO)
        {
            if (cuttingRecipeSO.inputKitchenObjectSO == inputKitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}

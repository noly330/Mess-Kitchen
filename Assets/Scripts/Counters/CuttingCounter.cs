using System;
using UnityEngine;

public class CuttingCounter : BaseCounter,IHasProgress
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
                player.GetKitchenObject().SetKitchenObjectParent(this);
                _cuttingProgress = 0;

                CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() { progressNormalized = ((float)_cuttingProgress) / cuttingRecipeSO.cuttingProgressMax });
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
                        GetKitchenObject().DestroySelf();
                    }
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            //TODO: 切割东西
            _cuttingProgress++;
            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() { progressNormalized = ((float)_cuttingProgress) / cuttingRecipeSO.cuttingProgressMax });
            if (_cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {

                KitchenObjectSO outputKitchenObjectSO = GetOutPutForInput(GetKitchenObject().GetKitchenObjectSO());

                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);

            }
            //_kitchenObject.Spawn
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

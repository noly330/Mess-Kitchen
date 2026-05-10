
using System.Data.Common;
using UnityEngine;
public class ClearCounter : BaseCounter
{

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                //TODO: 玩家没拿任何东西
            }
        }
        else
        {
            //如果玩家手上有东西
            if (player.HasKitchenObject())
            {
                //如果玩家有盘子
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))  
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestoryKitchenObject(GetKitchenObject());
                    }
                }
                else  //如果玩家没有盘子
                {
                    //如果柜台上有盘子
                    if(GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        //尝试把玩家受伤的东西放到柜台的盘子上
                        if(plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            KitchenObject.DestoryKitchenObject(player.GetKitchenObject());
                        }
                    }
                }
            }
            else  
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}

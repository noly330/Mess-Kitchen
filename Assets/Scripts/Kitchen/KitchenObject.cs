using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO _kitchenObjectSO;
    private IKitchenObjectParent _kitchenObjectParent;  //当前所在的柜台

    public KitchenObjectSO GetKitchenObjectSO() => _kitchenObjectSO;
    
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        if(this._kitchenObjectParent != null)
        {
            //先清理自己当前的_clearCounter
            this._kitchenObjectParent.ClearKitchenObject();
        }
        //设置新的_clearCounter
        _kitchenObjectParent = kitchenObjectParent;

        //TODO感觉有问题，以后要注意一下，因为现在是先设置它为父级再去检查父级有没有物品
        if (_kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError("The kitchen object parent already has a kitchen object");
        }
        _kitchenObjectParent.SetKitchenObject(this);
        transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public IKitchenObjectParent GetKitchenObjectParent() => _kitchenObjectParent;

    public void DestroySelf()
    {
        _kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if(this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else
        {
            plateKitchenObject = null;
            return false;
        }
    }

    /// <summary>
    /// 静态方法，用于生成厨房物品
    /// </summary>
    /// <param name="kitchenObjectSO"></param>
    /// <param name="kitchenObjectParent"></param>
    /// <returns></returns>
    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO,IKitchenObjectParent kitchenObjectParent)
    {
        Transform kitchenObejctTransform = Instantiate(kitchenObjectSO.prefab);
        KitchenObject kitchenObejct =  kitchenObejctTransform.GetComponent<KitchenObject>();
        kitchenObejct.SetKitchenObjectParent(kitchenObjectParent);
        return kitchenObejct;
    }
}

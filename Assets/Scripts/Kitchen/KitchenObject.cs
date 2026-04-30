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
        if (_kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError("尝试获取该物品的父级已经有厨房物品了");
        }
        _kitchenObjectParent.SetKitchenObject(this);
        transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public IKitchenObjectParent GetKitchenObjectParent() => _kitchenObjectParent;

    public void DestorySelf()
    {
        _kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);
    }

    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO,IKitchenObjectParent kitchenObjectParent)
    {
        Transform kitchenObejctTransform = Instantiate(kitchenObjectSO.prefab);
        KitchenObject kitchenObejct =  kitchenObejctTransform.GetComponent<KitchenObject>();
        kitchenObejct.SetKitchenObjectParent(kitchenObjectParent);
        return kitchenObejct;
    }
}

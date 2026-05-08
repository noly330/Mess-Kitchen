using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO _kitchenObjectSO;
    private FollowTransform _followTransform;
    private IKitchenObjectParent _kitchenObjectParent;  //当前所在的柜台
    public IKitchenObjectParent GetKitchenObjectParent() => _kitchenObjectParent;

    public KitchenObjectSO GetKitchenObjectSO() => _kitchenObjectSO;

    protected virtual void Awake()
    {
        _followTransform = GetComponent<FollowTransform>();
    }
    
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        SetKitchenoObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenoObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        SetKitchenObejctParentClientRpc(kitchenObjectParentNetworkObjectReference);
    }

    [ClientRpc]
    private void SetKitchenObejctParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();


        if(this._kitchenObjectParent != null)
        {
            //先清理自己当前的_clearCounter
            this._kitchenObjectParent.ClearKitchenObject();
        }
        //设置新的_clearCounter
        _kitchenObjectParent = kitchenObjectParent;

        //TODO感觉有问题，以后要注意一下，因为现在是先设置它为父级再去检查父级有没有物品
        if (kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError("The kitchen object parent already has a kitchen object");
        }
        kitchenObjectParent.SetKitchenObject(this);
        
        _followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
    }


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
    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO,IKitchenObjectParent kitchenObjectParent)
    {
        GameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSO,kitchenObjectParent);

    }
}

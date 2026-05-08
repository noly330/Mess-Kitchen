using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameMultiplayer : NetworkBehaviour
{
    public static GameMultiplayer Instance{get ; private set;}

    [SerializeField] private KitchenObjectListSO _kitchenObjectListSO;

    private  void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 静态方法，用于生成厨房物品
    /// </summary>
    /// <param name="kitchenObjectSO"></param>
    /// <param name="kitchenObjectParent"></param>
    /// <returns></returns>
    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO,IKitchenObjectParent kitchenObjectParent)
    {

        SpawnKitchenObjectServerRpc(GetKitchenObjectIndex(kitchenObjectSO),kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectIndex,NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjectSO kitchenObejctSO = GetKitchenObjectSO(kitchenObjectIndex);
        Transform kitchenObejctTransform = Instantiate(kitchenObejctSO.prefab);
        NetworkObject networkObject = kitchenObejctTransform.GetComponent<NetworkObject>();
        networkObject.Spawn(true);

        KitchenObject kitchenObejct =  kitchenObejctTransform.GetComponent<KitchenObject>();

        // 从网络对象引用中获取父对象
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        kitchenObejct.SetKitchenObjectParent(kitchenObjectParent);
    }

    private int GetKitchenObjectIndex(KitchenObjectSO kitchenObjectSO)
    {
        return _kitchenObjectListSO.kitchenObjectSOs.IndexOf(kitchenObjectSO);
    }

    private KitchenObjectSO GetKitchenObjectSO(int kitchenObjectIndex)
    {
        return _kitchenObjectListSO.kitchenObjectSOs[kitchenObjectIndex];
    }
}

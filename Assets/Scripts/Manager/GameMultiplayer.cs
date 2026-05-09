using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// 游戏多玩家管理类
/// 用于管理游戏中的多玩家同步
/// 包括生成厨房物品
/// </summary>
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
        networkObject.Spawn(true);  //在所有客户端生成一个网络对象

        KitchenObject kitchenObejct =  kitchenObejctTransform.GetComponent<KitchenObject>();

        // 从网络对象引用中获取父对象
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        //设置跟随对象，在这里有网络同步跟随的方法，不懂就点进去看
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

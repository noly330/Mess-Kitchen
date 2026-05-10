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


    #region  生成厨房物品
    /// <summary>
    /// 静态方法，用于生成厨房物品
    /// </summary>
    /// <param name="kitchenObjectSO"></param>
    /// <param name="kitchenObjectParent"></param>
    /// <returns></returns>
    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO,IKitchenObjectParent kitchenObjectParent)
    {

        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO),kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectIndex,NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjectSO kitchenObejctSO = GetKitchenObjectSOFromIndex(kitchenObjectIndex);
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

    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
    {
        return _kitchenObjectListSO.kitchenObjectSOs.IndexOf(kitchenObjectSO);
    }

    public KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectIndex)
    {
        return _kitchenObjectListSO.kitchenObjectSOs[kitchenObjectIndex];
    }
    #endregion
    #region  销毁厨房物品

    public void DestoryKitchenObejct(KitchenObject kitchenObject)
    {
        DestoryKitchenObejctServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestoryKitchenObejctServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        //先在所有客户端中清理父子绑定关系
        ClearKitchenObjectOnParentClientRpc(kitchenObjectNetworkObjectReference);

        //只有在服务器端才能销毁网络对象
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        kitchenObject.ClearKitchenObjectOnParent();
    }

    #endregion
}

using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏多玩家管理类
/// 用于管理游戏中的多玩家同步
/// 包括生成厨房物品
/// </summary>
public class GameMultiplayer : NetworkBehaviour
{
    public const int MAX_PLAYER_AMOUNT = 4;
    public const string PLAYER_PREFS_PLAYER_NAME_MULITIPLAYER = "PlayerBameMultiplayer";
    public static GameMultiplayer Instance { get; private set; }
    public static bool _playMultiplayer;
    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    [SerializeField] private KitchenObjectListSO _kitchenObjectListSO;
    [SerializeField] private List<Color> _playerColorList;
    private NetworkList<PlayerData> _playerDataNetworkList;
    private string _playerName;
    public string GetPlayerName() => _playerName;
    public void SetPlayerName(string playerName)
    {
        _playerName = playerName;
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULITIPLAYER, playerName);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        _playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULITIPLAYER, "PlayerName" + UnityEngine.Random.Range(100, 1000));
        _playerDataNetworkList = new NetworkList<PlayerData>();

        _playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;

        if (!_playMultiplayer)
        {
            StartCoroutine(StartSinglePlayerAfterInit());
        }
    }

    private System.Collections.IEnumerator StartSinglePlayerAfterInit()
    {
        yield return null;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", 7777);
        StartHost();
        Loader.LoadNetwork(Loader.Scene.GameScene);
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {

        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_Server_ConnectionApprovalCallback;

        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }


    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;

        NetworkManager.Singleton.StartClient();
    }


    private void NetworkManager_Server_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequese, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "游戏已经开始了";
            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "玩家已经满了";
            return;
        }
        connectionApprovalResponse.Approved = true;
        // if (GameManager.Instance.IsWaitingToStart())
        // {
        //     connectionApprovalResponse.CreatePlayerObject = true;
        //     connectionApprovalResponse.Approved = true;
        // }
        // else
        // {
        //     connectionApprovalResponse.Approved = false;
        // }

    }

    /// <summary>
    /// 只能有服务器执行，服务器添加了新的玩家数据到网络列表中，因为这是网络变量，会自动同步到所有客户端，所以客户端不需要执行
    /// </summary>
    /// <param name="clientID"></param>
    private void NetworkManager_Server_OnClientConnectedCallback(ulong clientID)
    {
        _playerDataNetworkList.Add(new PlayerData()
        {
            clientID = clientID,
            colorID = GetFirstUnusedColorID(),
        });

        SetPlayerNameServerRpc(_playerName);
        SetPlayerIDServerRpc(AuthenticationService.Instance.PlayerId);
    }
    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientID)
    {
        for (int i = 0; i < _playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = _playerDataNetworkList[i];
            if (playerData.clientID == clientID)
            {
                _playerDataNetworkList.RemoveAt(i);
                break;
            }
        }
    }
    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientID)
    {
        SetPlayerNameServerRpc(_playerName);
        SetPlayerIDServerRpc(AuthenticationService.Instance.PlayerId);
    }
    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientID)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientID(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = _playerDataNetworkList[playerDataIndex];
        playerData.playerName = playerName;
        _playerDataNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIDServerRpc(string playerID, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientID(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = _playerDataNetworkList[playerDataIndex];
        playerData.playerID = playerID;
        _playerDataNetworkList[playerDataIndex] = playerData;
    }



    #region  生成厨房物品
    /// <summary>
    /// 静态方法，用于生成厨房物品
    /// </summary>
    /// <param name="kitchenObjectSO"></param>
    /// <param name="kitchenObjectParent"></param>
    /// <returns></returns>
    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {

        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjectSO kitchenObejctSO = GetKitchenObjectSOFromIndex(kitchenObjectIndex);


        // 从网络对象引用中获取父对象
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        if(kitchenObjectParent.HasKitchenObject()) return;  //防止延迟高的时候玩家狂点，导致生成多个物品

        Transform kitchenObejctTransform = Instantiate(kitchenObejctSO.prefab);

        NetworkObject networkObject = kitchenObejctTransform.GetComponent<NetworkObject>();
        networkObject.Spawn(true);  //在所有客户端生成一个网络对象

        KitchenObject kitchenObejct = kitchenObejctTransform.GetComponent<KitchenObject>();

        

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

        if(kitchenObjectNetworkObject == null) return;  //防止延迟高的时候玩家狂点
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

    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < _playerDataNetworkList.Count;
    }

    #region PlayerData管理

    public int GetPlayerDataIndexFromClientID(ulong clientID)
    {
        for (int i = 0; i < _playerDataNetworkList.Count; i++)
        {
            if (_playerDataNetworkList[i].clientID == clientID)
            {
                return i;
            }
        }
        return -1;
    }
    public PlayerData GetPlayerDataFromClientID(ulong clientID)
    {
        foreach (PlayerData playerData in _playerDataNetworkList)
        {
            if (playerData.clientID == clientID)
            {
                return playerData;
            }
        }
        return default;
    }

    public PlayerData GetLocalPlayerData()
    {
        return GetPlayerDataFromClientID(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return _playerDataNetworkList[playerIndex];
    }

    #endregion

    #region 玩家颜色管理

    public Color GetPlayerColor(int colorID)
    {
        return _playerColorList[colorID];
    }

    public void ChangePlayerColor(int colorID)
    {
        ChangePlayerColorServerRpc(colorID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorID, ServerRpcParams serverRpcParams = default)
    {
        if (!IsColorAvailable(colorID))
        {
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientID(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = _playerDataNetworkList[playerDataIndex];

        playerData.colorID = colorID;
        _playerDataNetworkList[playerDataIndex] = playerData;

    }

    private bool IsColorAvailable(int colorID)
    {
        foreach (PlayerData playerData in _playerDataNetworkList)
        {
            if (playerData.colorID == colorID)
            {
                return false;
            }
        }
        return true;
    }

    //获得首个可用的颜色ID
    private int GetFirstUnusedColorID()
    {
        for (int i = 0; i < _playerColorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }
        return -1;
    }
    #endregion

    #region 玩家踢出管理

    public void KickPlayer(ulong clientID)
    {
        NetworkManager.Singleton.DisconnectClient(clientID);
        NetworkManager_Server_OnClientDisconnectCallback(clientID);
    }

    #endregion
}

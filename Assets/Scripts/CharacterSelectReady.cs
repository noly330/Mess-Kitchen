using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance;

    public event EventHandler OnReadyChanged;
    private Dictionary<ulong, bool> _playerReadyDic;

    private void Awake()
    {
        Instance = this;
        _playerReadyDic = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        //ServerRpcParams 是可选的网络参数，你可以用它来获取是谁发起了这个 RPC 调用
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        _playerReadyDic[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!_playerReadyDic.ContainsKey(clientID) || _playerReadyDic[clientID] == false)
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            GameLobby.Instance.DeleteLobby();  //服务端删除房间
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
        Debug.Log($"allClientsReady: {allClientsReady}");
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientID)
    {
        _playerReadyDic[clientID] = true;
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientID)
    {
        return _playerReadyDic.ContainsKey(clientID) && _playerReadyDic[clientID];
    }
}

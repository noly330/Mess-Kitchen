using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;
    [SerializeField] private KitchenObjectSO _plateKitchenObjectSO;
    [SerializeField] private float _spawnPlateTimerMax = 4f;


    private float _spawnPlateTimer;
    private int _platesSpawnedAmount;
    private int _maxPlatesSpawnedAmount = 4;


    private void Update()
    {
        if(!IsServer) return;
        _spawnPlateTimer += Time.deltaTime;
        if (_spawnPlateTimer >= _spawnPlateTimerMax && _platesSpawnedAmount < _maxPlatesSpawnedAmount && GameManager.Instance.IsGamePlaying())
        {
            SpawnPlateServerRpc();
        }
    }

    public override void Interact(Player player)
    {
        if(!player.HasKitchenObject() && _platesSpawnedAmount > 0)
        {
            KitchenObject.SpawnKitchenObject(_plateKitchenObjectSO, player);  //这个生成代码已经解决了网络同步，所以让它在这生成把
            InteractLogicServerRpc();
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc()  //其实可以不用这个，但是为了保持一致性和可读性，还是用吧
    {
        SpawnPlateClientRpc();
    }
    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        //KitchenObject.SpawnKitchenObject(_plateKitchenObjectSO, this);
            _platesSpawnedAmount++;
            _spawnPlateTimer = 0;
            OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }


    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        _platesSpawnedAmount--;
            OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}

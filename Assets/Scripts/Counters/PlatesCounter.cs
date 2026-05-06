using System;
using System.Collections.Generic;
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
        _spawnPlateTimer += Time.deltaTime;
        if (_spawnPlateTimer >= _spawnPlateTimerMax && _platesSpawnedAmount < _maxPlatesSpawnedAmount && GameManager.Instance.IsGamePlaying())
        {
            //KitchenObject.SpawnKitchenObject(_plateKitchenObjectSO, this);
            _platesSpawnedAmount++;
            _spawnPlateTimer = 0;
            OnPlateSpawned?.Invoke(this, EventArgs.Empty);
        }
    }

    public override void Interact(Player player)
    {
        if(!player.HasKitchenObject() && _platesSpawnedAmount > 0)
        {
            KitchenObject.SpawnKitchenObject(_plateKitchenObjectSO, player);
            _platesSpawnedAmount--;
            OnPlateRemoved?.Invoke(this, EventArgs.Empty);
        }
    }
}

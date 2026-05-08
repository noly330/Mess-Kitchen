using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    public static DeliveryManager Instance;
    [SerializeField] private RecipeListSO _recipeListSO;  // 配方列表
    private List<RecipeSO> _waitingRecipeSOList = new List<RecipeSO>();  // 等待配方列表

    private float _spawnRecipetimer = 4f;
    [SerializeField] private float _spawnRecipetimerMax = 10f;
    private int _waitingRecipeMax = 4;  // 最大等待配方数
    private int _successfulRecipesAmount = 0; // 成功递送的配方数
    public int GetSuccessfulRecipesAmount() => _successfulRecipesAmount;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (!IsServer) return;

        _spawnRecipetimer -= Time.deltaTime;
        if (_spawnRecipetimer <= 0)
        {
            _spawnRecipetimer = _spawnRecipetimerMax;

            if (GameManager.Instance.IsGamePlaying() && _waitingRecipeSOList.Count < _waitingRecipeMax)
            {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, _recipeListSO.recipeSOList.Count);
                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);

            }
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {

        RecipeSO waitingRecipeSO = _recipeListSO.recipeSOList[waitingRecipeSOIndex];
        _waitingRecipeSOList.Add(waitingRecipeSO);
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < _waitingRecipeSOList.Count; i++)  //第一层循环，遍历等待递送的配方列表
        {
            RecipeSO waitingRecipeSO = _waitingRecipeSOList[i];

            if (plateKitchenObject.GetKitchenObjectSOList().Count == waitingRecipeSO.kitchenObjectSOList.Count)
            {

                bool plateContentsMatchesRecipe = true;  //只要有一个没找到，就变为false
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)  //第二层循环，遍历配方中的食材
                {
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO platerKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())  //第三层循环，遍历盘中的食材是否包含配方中的食材
                    {
                        if (platerKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }

                    if (!ingredientFound)
                    {
                        plateContentsMatchesRecipe = false;
                        break;
                    }
                }

                if (plateContentsMatchesRecipe)  //如果所有食材都找到了，就递送成功
                {
                    DeliverCorrectRecipeServerRpc(i);
                    return;
                }
            }
        }

        DeliveryFailedRecipeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex)
    {
        DeliveryCorrectRecipeClientRpc(waitingRecipeSOListIndex);
    }

    [ClientRpc]
    private void DeliveryCorrectRecipeClientRpc(int waitingRecipeSOListIndex)
    {
        _successfulRecipesAmount++;
        _waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliveryFailedRecipeServerRpc()
    {
        DeliveryFailedRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliveryFailedRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return _waitingRecipeSOList;
    }
}

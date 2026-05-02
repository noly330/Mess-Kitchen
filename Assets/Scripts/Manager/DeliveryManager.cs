using System.Collections.Generic;
using UnityEngine;
using System;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance;
    [SerializeField] private RecipeListSO _recipeListSO;  // 配方列表
    private List<RecipeSO> _waitingRecipeSOList = new List<RecipeSO>();  // 等待配方列表

    private float _spawnRecipetimer;
    [SerializeField] private float _spawnRecipetimerMax = 10f;

    private int _waitingRecipeMax = 4;  // 最大等待配方数
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        _spawnRecipetimer -= Time.deltaTime;
        if (_spawnRecipetimer <= 0)
        {
            _spawnRecipetimer = _spawnRecipetimerMax;

            if (_waitingRecipeSOList.Count < _waitingRecipeMax)
            {

                RecipeSO waitingRecipeSO = _recipeListSO.recipeSOList[UnityEngine.Random.Range(0, _recipeListSO.recipeSOList.Count)];
                _waitingRecipeSOList.Add(waitingRecipeSO);
                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for(int i = 0; i < _waitingRecipeSOList.Count; i++)  //第一层循环，遍历等待递送的配方列表
        {
            RecipeSO waitingRecipeSO = _waitingRecipeSOList[i];

            if(plateKitchenObject.GetKitchenObjectSOList().Count == waitingRecipeSO.kitchenObjectSOList.Count)
            {

                bool plateContentsMatchesRecipe = true;  //只要有一个没找到，就变为false
                foreach(KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)  //第二层循环，遍历配方中的食材
                {
                    bool ingredientFound = false;
                    foreach(KitchenObjectSO platerKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())  //第三层循环，遍历盘中的食材是否包含配方中的食材
                    {
                        if(platerKitchenObjectSO == recipeKitchenObjectSO)
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
                    _waitingRecipeSOList.RemoveAt(i);
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }

        //TODO: 玩家交付了错误的配方
        Debug.Log("玩家交付了错误的配方");
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return _waitingRecipeSOList;
    }
}

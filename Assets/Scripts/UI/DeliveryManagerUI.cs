using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform _container;
    [SerializeField] private Transform _recipeTemplate;


    private void Awake()
    {
        _recipeTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSpawned += OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted += OnRecipeCompleted;
        UpdateVisual();
    }

    private void OnRecipeCompleted(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void OnRecipeSpawned(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach(Transform child in _container)
        {
            if(child != _recipeTemplate)
            {
                Destroy(child.gameObject);
            }
        }

        List<RecipeSO> waitingRecipeSOList = DeliveryManager.Instance.GetWaitingRecipeSOList();
        foreach(RecipeSO waitingRecipeSO in waitingRecipeSOList)
        {
            Transform recipeTransform = Instantiate(_recipeTemplate, _container);
            recipeTransform.gameObject.SetActive(true);
            recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(waitingRecipeSO);
        }
    }
}

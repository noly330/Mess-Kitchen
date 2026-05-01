using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
    private PlateKitchenObject _plateKitchenObject;
    [SerializeField] private Transform _iconTemplate;

    private void Awake()
    {
        _plateKitchenObject = GetComponentInParent<PlateKitchenObject>();
         _iconTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        _plateKitchenObject.OnIngredientAdded += OnIngredientAdded;
    }

    private void OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {

        foreach(Transform child in transform)
        {
            //对于教程的疑问：为什么不直接用预制体？第二个视频的1时22分
            if(child == _iconTemplate)
                continue;
            Destroy(child.gameObject);
        }


        List<KitchenObjectSO> kitchenObjectSOList = _plateKitchenObject.GetKitchenObjectSOList();
        foreach(KitchenObjectSO kitchenObjectSO in kitchenObjectSOList)
        {
            Transform iconTransform = Instantiate(_iconTemplate,transform);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<PlateIconsSingleUI>().SetKitchenObejctSO(kitchenObjectSO);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompaleteVisual : MonoBehaviour
{
    [System.Serializable]
    public struct KitchenObejctSO_GameObejct
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }
    private PlateKitchenObject _plateKitchenObject;
    [SerializeField] private List<KitchenObejctSO_GameObejct> _kitchenOSO_GameObejctList;

    private void Awake()
    {
        _plateKitchenObject = GetComponentInParent<PlateKitchenObject>();
    }   

    private void Start()
    {
        _plateKitchenObject.OnIngredientAdded += OnIngredientAdded;


        foreach(KitchenObejctSO_GameObejct kitchenObejctSO_GameObejct in _kitchenOSO_GameObejctList)
        {
            kitchenObejctSO_GameObejct.gameObject.SetActive(false);
        }
    }

    private void OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        foreach(KitchenObejctSO_GameObejct kitchenObejctSO_GameObejct in _kitchenOSO_GameObejctList)
        {
            if(kitchenObejctSO_GameObejct.kitchenObjectSO == e.kitchenObjectSO)
            {
                kitchenObejctSO_GameObejct.gameObject.SetActive(true);
            }
        }
    }
}

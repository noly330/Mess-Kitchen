using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private Transform _counterTopPoint;
    [SerializeField] private Transform plateVisualPrefab;

    private List<GameObject> _plateVistualGameObjectList;
    private PlatesCounter _platesCounter;

    private void Awake()
    {
        _platesCounter = GetComponentInParent<PlatesCounter>();
        _plateVistualGameObjectList = new List<GameObject>();
    }
    private void Start()
    {
        _platesCounter.OnPlateSpawned += OnPlateSpawned;
        _platesCounter.OnPlateRemoved += OnPlateRemoved;
    }


    private void OnPlateSpawned(object sender, EventArgs e)
    {
        Transform plateVisualTransform = Instantiate(plateVisualPrefab, _counterTopPoint);
        float plateOffsetY = 0.1f;
        plateVisualTransform.localPosition = new Vector3(0, plateOffsetY * _plateVistualGameObjectList.Count, 0);
        _plateVistualGameObjectList.Add(plateVisualTransform.gameObject);
    }
    private void OnPlateRemoved(object sender, EventArgs e)
    {
        GameObject plateGameObject = _plateVistualGameObjectList[_plateVistualGameObjectList.Count - 1];
        _plateVistualGameObjectList.RemoveAt(_plateVistualGameObjectList.Count - 1);
        Destroy(plateGameObject);
    }
}

using UnityEngine.UI;
using UnityEngine;
using System;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int _colorID;
    [SerializeField] private Image image;
    [SerializeField] private GameObject _selectedGameobject;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameMultiplayer.Instance.ChangePlayerColor(_colorID);
        });
    }
    private void Start()
    {
        GameMultiplayer.Instance.OnPlayerDataNetworkListChanged += GameMultiplayer_OnPlayerDataNetworkListChanged;
        image.color = GameMultiplayer.Instance.GetPlayerColor(_colorID);
        UpdateIsSelected();
    }

    private void GameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if(GameMultiplayer.Instance.GetLocalPlayerData().colorID == _colorID)
        {
            _selectedGameobject.SetActive(true);
        }
        else
        {
            _selectedGameobject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        GameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= GameMultiplayer_OnPlayerDataNetworkListChanged;
    }
}

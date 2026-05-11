using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int _playerIndex;
    [SerializeField] private GameObject _readyGameObject;
    [SerializeField] private Button _kickButton;
    private PlayerVisual _playerVisual;

    private void Awake()
    {
        _playerVisual = GetComponentInChildren<PlayerVisual>();

        _kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);
            GameMultiplayer.Instance.KickPlayer(playerData.clientID);
        });
    }
    private void Start()
    {
        GameMultiplayer.Instance.OnPlayerDataNetworkListChanged += GameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

        _kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        UpdatePlayer();
    }


    private void GameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }
    private void CharacterSelectReady_OnReadyChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (GameMultiplayer.Instance.IsPlayerIndexConnected(_playerIndex))
        {
            Show();
            PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);
            _readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientID));
            _playerVisual.SetPlayerColor(GameMultiplayer.Instance.GetPlayerColor(playerData.colorID));
        }
        else
        {
            Hide();

        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= GameMultiplayer_OnPlayerDataNetworkListChanged;
        //CharacterSelectReady.Instance.OnReadyChanged -= CharacterSelectReady_OnReadyChanged;
    }

}

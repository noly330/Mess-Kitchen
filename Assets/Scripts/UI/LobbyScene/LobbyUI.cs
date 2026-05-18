using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _createLobbyButton;
    [SerializeField] private Button _quickJoinButton;
    [SerializeField] private Button _joinLobbyButton;
    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private TMP_InputField _playerNameInputField;
    [SerializeField] private LobbyCreateUI _lobbyCreateUI;
    [SerializeField] private Transform _lobbyContainer;
    [SerializeField] private Transform _lobbyTemplate;



    private void Awake()
    {
        _mainMenuButton.onClick.AddListener(() =>
        {
            Loader.LoadScene(Loader.Scene.MainMenuScene);
            GameLobby.Instance.LeaveLobby();
        });
        _createLobbyButton.onClick.AddListener(() =>
        {
            _lobbyCreateUI.Show();
            //GameLobby.Instance.CreateLobby("LobbyName",false);
        });
        _quickJoinButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.QuickJoin();
        });

        _joinLobbyButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.JoinWithCode(_joinCodeInputField.text);
        });

        _lobbyTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        _playerNameInputField.text = GameMultiplayer.Instance.GetPlayerName();
        _playerNameInputField.onValueChanged.AddListener((string newText) =>
        {
            GameMultiplayer.Instance.SetPlayerName(newText);
        });

        GameLobby.Instance.OnLobbyListChanged += GameLobby_OnLobbyListChanged;
    }

    private void GameLobby_OnLobbyListChanged(object sender, GameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in _lobbyContainer)
        {
            if (child == _lobbyTemplate)
            {
                continue;
            }
            Destroy(child.gameObject);

        }
        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(_lobbyTemplate, _lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            LobbyListSingleUI lobbyListSingleUI = lobbyTransform.GetComponent<LobbyListSingleUI>();
            lobbyListSingleUI.SetLobby(lobby);
        }
    }

    private void OnDestroy()
    {
        GameLobby.Instance.OnLobbyListChanged -= GameLobby_OnLobbyListChanged;
    }
}

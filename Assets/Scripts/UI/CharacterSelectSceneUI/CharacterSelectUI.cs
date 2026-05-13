using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _readyButton;
    [SerializeField] private TextMeshProUGUI _lobbyNameText;
    [SerializeField] private TextMeshProUGUI _lobbyCodeText;

    private void Awake()
    {
        _mainMenuButton.onClick.AddListener(() =>
        {
            //安全地断开所有网络连接，并清理 NetworkManager 的单例实例
            NetworkManager.Singleton.Shutdown();
            GameLobby.Instance.LeaveLobby();
            Loader.LoadScene(Loader.Scene.MainMenuScene);
        });
        _readyButton.onClick.AddListener(() =>
        {
            CharacterSelectReady.Instance.SetPlayerReady();
        });
    }

    private void Start()
    {
        Lobby lobby = GameLobby.Instance.GetLobby();
        _lobbyNameText.text = "大厅名称:" + lobby.Name;
        _lobbyCodeText.text = "大厅代码:" + lobby.LobbyCode;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyNameText;
    private Lobby _lobby;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameLobby.Instance.JoinWithID(_lobby.Id);
        });
    }
    public void SetLobby(Lobby lobby)
    {
        _lobby = lobby;
        _lobbyNameText.text = lobby.Name;
    }
}

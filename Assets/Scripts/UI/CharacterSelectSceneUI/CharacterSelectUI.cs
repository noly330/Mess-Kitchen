using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _readyButton;

    private void Awake()
    {
        _mainMenuButton.onClick.AddListener(() =>
        {
            //安全地断开所有网络连接，并清理 NetworkManager 的单例实例
            NetworkManager.Singleton.Shutdown();
            Loader.LoadScene(Loader.Scene.MainMenuScene);
        });
        _readyButton.onClick.AddListener(() =>
        {
            CharacterSelectReady.Instance.SetPlayerReady();
        });
    }
}

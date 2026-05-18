using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _playSingleplayerButton;
    [SerializeField] private Button _playMultiplayerButton;
    [SerializeField] private Button _quitButton;

    private void Awake()
    {
        _playSingleplayerButton.onClick.AddListener(() =>
        {
            GameMultiplayer._playMultiplayer = false;
            Loader.LoadScene(Loader.Scene.LobbyScene);
        });
        _playMultiplayerButton.onClick.AddListener(() =>
        {
            GameMultiplayer._playMultiplayer = true;
            Loader.LoadScene(Loader.Scene.LobbyScene);
        });
        _quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        Time.timeScale = 1f;
    }
}

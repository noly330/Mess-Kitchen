using System;
using UnityEngine.UI;
using UnityEngine;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _resumeGameButton;

    private void Awake()
    {
        _mainMenuButton.onClick.AddListener(() =>
        {
            Loader.LoadScene(Loader.Scene.MainMenuScene);
        });
        _resumeGameButton.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePauseGame();
        });
    }

    private void Start()
    {
        GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
        GameManager.Instance.OnGameResumed += GameManager_OnGameResumed;

        Hide();
    }

    private void GameManager_OnGameResumed(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGamePaused(object sender, EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}

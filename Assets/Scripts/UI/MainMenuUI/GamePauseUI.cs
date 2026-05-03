using System;
using UnityEngine.UI;
using UnityEngine;

public class GamePauseUI : MonoBehaviour
{
    public static GamePauseUI Instance;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _resumeGameButton;
    [SerializeField] private Button _optionsButton;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _mainMenuButton.onClick.AddListener(() =>
        {
            Loader.LoadScene(Loader.Scene.MainMenuScene);
        });
        _resumeGameButton.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePauseGame();
        });
        _optionsButton.onClick.AddListener(() =>
        {
            OptionsUI.Instance.Show();
            Hide();
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

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}

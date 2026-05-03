using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameResumed;

    public static GameManager Instance;
    private enum State
    {
        WaitingToStart, CountdownToStart, GamePlaying, GameOver
    }

    private State _currentState;
    private float _waitingToStartTimer = 1f;
    private float _countdownToStartTimer = 3f;

    private float _gamePlayingTimerMax = 30f;
    private float _gamePlayingTimer;
    private bool _isPaused = false;

    private void Awake()
    {
        _currentState = State.WaitingToStart;
        Instance = this;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
    }


    private void Update()
    {
        switch (_currentState)
        {
            case State.WaitingToStart:
                _waitingToStartTimer -= Time.deltaTime;
                if (_waitingToStartTimer <= 0)
                {
                    _currentState = State.CountdownToStart;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.CountdownToStart:
                _countdownToStartTimer -= Time.deltaTime;
                if (_countdownToStartTimer <= 0)
                {
                    _gamePlayingTimer = _gamePlayingTimerMax;
                    _currentState = State.GamePlaying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                _gamePlayingTimer -= Time.deltaTime;
                if (_gamePlayingTimer <= 0)
                {
                    _currentState = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:

                break;

        }
        Debug.Log(_currentState);
    }

    public bool IsGamePlaying()
    {
        return _currentState == State.GamePlaying;
    }
    public bool IsCountdownToStartActive()
    {
        return _currentState == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return _countdownToStartTimer;
    }
    public bool IsGameOver()
    {
        return _currentState == State.GameOver;
    }

    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (_gamePlayingTimer / _gamePlayingTimerMax);
    }
    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    public void TogglePauseGame()
    {
        if (_isPaused)
        {
            Time.timeScale = 1f;
            _isPaused = false;
            OnGameResumed?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 0;
            _isPaused = true;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
    
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    
    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameResumed;
    public event EventHandler OnLoaclPlayerReadyChanged;

    public static GameManager Instance;
    private enum GameState
    {
        WaitingToStart, CountdownToStart, GamePlaying, GameOver

    }
    private bool _isLocalPlayerReady;
    public bool IsLocalPlayerReady => _isLocalPlayerReady;

    private NetworkVariable<GameState> _currentState = new NetworkVariable<GameState>(GameState.WaitingToStart);
    private NetworkVariable<float> _countdownToStartTimer = new NetworkVariable<float>(3f);

    private NetworkVariable<float> _gamePlayingTimer = new NetworkVariable<float>(0f);
    private float _gamePlayingTimerMax = 30f;
    private bool _isPaused = false;

    private Dictionary<ulong,bool> _playerReadyDic;

    private void Awake()
    {
        Instance = this;
        _playerReadyDic = new Dictionary<ulong,bool>();
    }


    public override void OnNetworkSpawn()
    {
        _currentState.OnValueChanged += GameState_OnValueChanged;
    }

    private void GameState_OnValueChanged(GameState previousValue, GameState newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerReadyDic[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach(ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if(!_playerReadyDic.ContainsKey(clientID) || _playerReadyDic[clientID] == false)
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            _currentState.Value = GameState.CountdownToStart;
        }
        Debug.Log($"allClientsReady: {allClientsReady}");
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if(_currentState.Value == GameState.WaitingToStart)
        {
            _isLocalPlayerReady = true;
            OnLoaclPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
            // _currentState = State.CountdownToStart;
            // OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Update()
    {
        if(!IsServer)  return;

        switch (_currentState.Value)
        {
            case GameState.WaitingToStart:
                // _waitingToStartTimer -= Time.deltaTime;
                // if (_waitingToStartTimer <= 0)
                // {
                //     _currentState = State.CountdownToStart;
                //     OnStateChanged?.Invoke(this, EventArgs.Empty);
                // }
                break;
            case GameState.CountdownToStart:
                _countdownToStartTimer.Value -= Time.deltaTime;
                if (_countdownToStartTimer.Value <= 0)
                {
                    _gamePlayingTimer.Value = _gamePlayingTimerMax;
                    _currentState.Value = GameState.GamePlaying;
                    
                }
                break;
            case GameState.GamePlaying:
                _gamePlayingTimer.Value -= Time.deltaTime;
                if (_gamePlayingTimer.Value <= 0)
                {
                    _currentState.Value = GameState.GameOver;
                    
                }
                break;
            case GameState.GameOver:

                break;

        }
    }

    public bool IsGamePlaying()
    {
        return _currentState.Value == GameState.GamePlaying;
    }
    public bool IsCountdownToStartActive()
    {
        return _currentState.Value == GameState.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return _countdownToStartTimer.Value;
    }
    public bool IsGameOver()
    {
        return _currentState.Value == GameState.GameOver;
    }

    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (_gamePlayingTimer.Value / _gamePlayingTimerMax);
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

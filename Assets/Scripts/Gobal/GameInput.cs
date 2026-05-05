using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";
    public static GameInput Instance { get; private set; }

    private PlayerInputControls _playerInputControls;

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;

    public enum Binding
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        InteractAlternate,
        Pause,
    }
    private void Awake()
    {
        Instance = this;
        _playerInputControls = new PlayerInputControls();
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            _playerInputControls.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
        _playerInputControls.Enable();

        _playerInputControls.Player.Interact.performed += OnInteractPerformed;
        _playerInputControls.Player.InteractAlternate.performed += OnInteractAlternatePerformed;
        _playerInputControls.Player.Pause.performed += OnPausePerformed;

    }

    private void OnDestroy()
    {
        _playerInputControls.Player.Interact.performed -= OnInteractPerformed;
        _playerInputControls.Player.InteractAlternate.performed -= OnInteractAlternatePerformed;
        _playerInputControls.Player.Pause.performed -= OnPausePerformed;

        _playerInputControls.Dispose();
    }


    public Vector2 GetMovementDirectionNormalized()
    {
        Vector2 inputDirection = _playerInputControls.Player.Move.ReadValue<Vector2>();

        inputDirection = inputDirection.normalized;
        return inputDirection;
    }
    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }
    private void OnInteractAlternatePerformed(InputAction.CallbackContext context)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }
    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }


    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
            case Binding.MoveUp:
                return _playerInputControls.Player.Move.bindings[1].ToDisplayString();
            case Binding.MoveDown:
                return _playerInputControls.Player.Move.bindings[2].ToDisplayString();
            case Binding.MoveLeft:
                return _playerInputControls.Player.Move.bindings[3].ToDisplayString();
            case Binding.MoveRight:
                return _playerInputControls.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return _playerInputControls.Player.Interact.bindings[0].ToDisplayString();
            case Binding.InteractAlternate:
                return _playerInputControls.Player.InteractAlternate.bindings[0].ToDisplayString();
            case Binding.Pause:
                return _playerInputControls.Player.Pause.bindings[0].ToDisplayString();

        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        _playerInputControls.Player.Disable();

        InputAction inputAction;
        int bindingIndex;

        switch (binding)
        {
            default:
            case Binding.MoveUp:
                inputAction = _playerInputControls.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.MoveDown:
                inputAction = _playerInputControls.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.MoveLeft:
                inputAction = _playerInputControls.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.MoveRight:
                inputAction = _playerInputControls.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = _playerInputControls.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlternate:
                inputAction = _playerInputControls.Player.InteractAlternate;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = _playerInputControls.Player.Pause;
                bindingIndex = 0;
                break;

        }
        inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback =>
        {
            callback.Dispose();  //新版本输入系统其实不需要手动释放资源，反正先写上无所谓吧
            _playerInputControls.Player.Enable();
            onActionRebound?.Invoke();

            PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, _playerInputControls.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();
        }).Start();

    }
}

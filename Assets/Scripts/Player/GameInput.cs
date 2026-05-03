using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private PlayerInputControls _playerInputControls;

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;
    private void Awake()
    {
        Instance = this;
        _playerInputControls = new PlayerInputControls();
        _playerInputControls.Enable();

        _playerInputControls.Player.Interact.performed += OnInteractPerformed;
        _playerInputControls.Player.InteractAlternate.performed += OnInteractAlternatePerformed;
        _playerInputControls.Player.Pause.performed += OnPausePerformed;
    }

        private void OnDestroy() {
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
}

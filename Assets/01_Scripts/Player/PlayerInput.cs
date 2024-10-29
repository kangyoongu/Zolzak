using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "SO/InputTest")]
public class PlayerInput : ScriptableObject
{
    private Controls _inputAction;
    public Controls InputAction => _inputAction;

    public event Action<Vector2> OnMovement;
    public event Action<Vector2> OnAim;
    public event Action OnJump;
    public event Action ClickEsc;
    public event Action OnShift;
    public Vector2 Movement { get; private set; }
    public bool Shift { get; private set; }
    public bool DownJump { get; private set; }

    private void OnEnable()
    {
        _inputAction = new Controls();
        _inputAction.Playing.Enable();
        _inputAction.Playing.Mouse.performed += Aim_performed;
        _inputAction.Playing.Move.performed += Movement_performed;
        _inputAction.Playing.Jump.performed += Jump_performed;
        _inputAction.Playing.Move.canceled += Movement_performed;
        _inputAction.Playing.Shift.performed += (obj) => Shift = true;
        _inputAction.Playing.Shift.canceled += (obj) => Shift = false;
        _inputAction.Playing.Jump.canceled += (obj) => DownJump = false;
    }
    private void Movement_performed(InputAction.CallbackContext obj)
    {
        Movement = obj.ReadValue<Vector2>();
    }
    private void Aim_performed(InputAction.CallbackContext obj)
    {
        OnAim?.Invoke(obj.ReadValue<Vector2>());
    }
    private void Jump_performed(InputAction.CallbackContext obj)
    {
        DownJump = true;
        OnJump?.Invoke();
    }
    private void OnDisable()
    {
        _inputAction.Playing.Disable();
    }
}

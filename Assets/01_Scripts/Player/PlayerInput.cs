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

    public event Action<Vector2> OnAim;
    public event Action DownJump;
    public event Action UpJump;
    public event Action ClickEsc;
    public event Action DownCtrl;
    public event Action UpCtrl;
    public event Action DownShift;
    public event Action UpShift;
    public event Action LCDown;
    public event Action LCUp;
    public Vector2 Movement { get; private set; }
    public bool Shift { get; private set; }
    public bool Jumping { get; private set; }

    private void OnEnable()
    {
        _inputAction = new Controls();
        _inputAction.Playing.Enable();
        _inputAction.Playing.Mouse.performed += Aim_performed;
        _inputAction.Playing.Move.performed += Movement_performed;
        _inputAction.Playing.Move.canceled += Movement_performed;
        _inputAction.Playing.Jump.performed += Jump_performed;
        _inputAction.Playing.Jump.canceled += Jump_canceled;
        _inputAction.Playing.Shift.performed += Shift_performed;
        _inputAction.Playing.Shift.canceled += Shift_canceled;
        _inputAction.Playing.Click.performed += (obj) => LCDown?.Invoke();
        _inputAction.Playing.Click.canceled += (obj) => LCUp?.Invoke();

        _inputAction.Playing.Ctrl.performed += (obj) => DownCtrl?.Invoke();
        _inputAction.Playing.Ctrl.canceled += (obj) => UpCtrl?.Invoke();
    }

    private void Shift_canceled(InputAction.CallbackContext obj)
    {
        UpShift?.Invoke();
        Shift = false;
    }

    private void Shift_performed(InputAction.CallbackContext obj)
    {
        DownShift?.Invoke();
        Shift = true;
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        Jumping = false;
        UpJump?.Invoke();
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
        Jumping = true;
        DownJump?.Invoke();
    }
    private void OnDisable()
    {
        _inputAction.Playing.Disable();
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParkour : MonoBehaviour
{
    [SerializeField] private List<Parkour> _parkours;
    [SerializeField] private List<ActionState> _actions;
    Player _player;

    Parkour _currentParkour;
    private void Awake()
    {
        _player = GetComponent<Player>();
    }
    private void Start()
    {
        foreach (Parkour parkour in _parkours)
        {
            parkour.Init(_player);
        }
        foreach (ActionState action in _actions)
        {
            action.Init(_player);
        }
        _player.playerInput.DownJump += CheckAction;
    }
    private void OnDestroy()
    {

        _player.playerInput.DownJump -= CheckAction;
    }
    private void CheckAction()
    {
        if (_player.parkouring) return;

        Vector3 localVelocity = transform.InverseTransformDirection(_player.Rigidbody.linearVelocity);
        float speed = localVelocity.z;

        foreach (Parkour parkour in _parkours)
        {
            if (parkour.ActionCondition(transform, speed, ref _currentParkour))
            {
                StartAnim();
                return;
            }
        }

        if (_player.playerMovement.grounded)
            _player.playerMovement.Jump();
    }

    private void StartAnim()
    {
        _currentParkour.Play();
        _player.StartPhysics();
    }

    public void SetPos()
    {
        _currentParkour.SetPos();
    }

    public void EndAnim()
    {
        _currentParkour.EndAnim();
        _player.EndPhysics();
    }
    public void StartAction(string name)
    {
        foreach (ActionState action in _actions)
        {
            if(name == action.gameObject.name)
            {
                action.StartAction();
                return;
            }
        }
    }
    public void EndAction(string name)
    {

        foreach (ActionState action in _actions)
        {
            if (name == action.gameObject.name)
            {
                action.EndAction();
                return;
            }
        }
    }
}

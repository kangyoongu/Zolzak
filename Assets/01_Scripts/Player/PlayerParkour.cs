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
        _player.playerInput.OnJump += () =>
        {
            Vector3 localVelocity = transform.InverseTransformDirection(_player.Rigidbody.linearVelocity);
            CheckAction(localVelocity);
        };
    }
    private void CheckAction(Vector3 velocity)
    {
        if (!_player.playerMovement.grounded) return;
        if (velocity.z < 9f)
        {
            _player.playerMovement.Jump();
            return;
        } 
        
        foreach (Parkour parkour in _parkours)
        {
            if (parkour.ActionCondition(transform, ref _currentParkour))
            {
                StartAnim();
                return;
            }
        }

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

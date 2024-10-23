using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParkour : MonoBehaviour
{
    [SerializeField] private List<Parkour> _parkours;
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
        _player.playerInput.OnJump += () =>
        {
            Vector3 localVelocity = transform.InverseTransformDirection(_player.Rigidbody.linearVelocity);
            CheckAction(localVelocity);
        };
    }
    private void CheckAction(Vector3 velocity)
    {
        if (velocity.z < 9f) return; 
        
        foreach (Parkour parkour in _parkours)
        {
            if (parkour.ActionCondition(transform))
            {
                StartAnim(parkour);
                return;
            }
        }

        _player.playerMovement.Jump();
    }

    private void StartAnim(Parkour parkour)
    {
        parkour.Play();
        _currentParkour = parkour;

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
}

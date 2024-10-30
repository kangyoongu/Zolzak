using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class ActionState : MonoBehaviour
{
    protected Player _player;
    protected bool _actioning = false;
    public virtual void Init(Player player)
    {
        _player = player;
    }

    public virtual void StartAction()
    {
        _player.StartPhysics();
        _actioning = true;
    }

    public virtual void EndAction()
    {
        _player.EndPhysics();
        _actioning = false;
    }
}

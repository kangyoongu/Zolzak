using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Roll : ActionState
{
    [SerializeField] float _speed;
    [SerializeField] MultiParentConstraint _multiParent;
    private void FixedUpdate()
    {
        if (_actioning)
        {
            Vector3 dir = new Vector3(_player.transform.forward.x * _speed, _player.Rigidbody.linearVelocity.y, _player.transform.forward.z * _speed);
            _player.Rigidbody.linearVelocity = dir;
        }
    }
    public override void StartAction()
    {
        DOTween.To(() => _multiParent.weight, x => _multiParent.weight = x, 0f, 0.4f);
        base.StartAction();
        _player.Rigidbody.isKinematic = false;
    }
    public override void EndAction()
    {
        DOTween.To(() => _multiParent.weight, x => _multiParent.weight = x, 1f, 0.4f);
        base.EndAction();
    }
}

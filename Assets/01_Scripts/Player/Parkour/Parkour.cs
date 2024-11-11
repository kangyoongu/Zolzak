using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Parkour : MonoBehaviour
{
    protected Player _player;
    private int _nameHash;
    public List<Moving> movePos;
    [HideInInspector] public List<Vector3> originPos;
    int _index = 0;
    [SerializeField] protected LayerMask castLayer;

    Tweener tweener;
    public virtual void Init(Player player)
    {
        _nameHash = Animator.StringToHash(gameObject.name);
        originPos = new List<Vector3>();
        _player = player;
    }
    public abstract bool ActionCondition(Transform player, float speed, ref Parkour parkour);
    public void Play()
    {
        _player.playerAnim.anim.SetTrigger(_nameHash);
    }

    public void SetPos()
    {
        if (movePos[_index].duration == -1f)
        {
            if (tweener != null && tweener.IsActive()) tweener.Kill();

            _player.Rigidbody.isKinematic = false;
            _player.Rigidbody.AddRelativeForce(movePos[_index].pos, ForceMode.Force);
        }
        else
        {
            _player.Rigidbody.isKinematic = true;
            tweener = _player.transform.DOMove(originPos[_index] + _player.transform.TransformDirection(movePos[_index].pos), movePos[_index].duration).SetEase(Ease.Linear);
        }
        _index++;
    }

    public virtual void EndAnim()
    {
        originPos = new List<Vector3>();
        _index = 0;
    }
}

[Serializable]
public struct Moving
{
    public Vector3 pos;
    public float duration;
}
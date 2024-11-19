using DG.Tweening;
using System;
using UnityEngine;

public class Pipe : ActionState
{
    public float hangDistance = 2.0f;  // SphereCast의 최대 거리
    public float offsetY = 2f;// 좌표 오프셋 값
    public float speed = 10f;
    public float sideMoveDis = 1f;
    [SerializeField] Vector3 _hangOffset;
    [SerializeField] float _duration;
    [SerializeField] float _hangDuratdion;
    [SerializeField] LayerMask _climbLayer;
    bool _isClimb = false;
    bool _movable = true;
    Vector3 _offsetY;
    public override void Init(Player player)
    {
        base.Init(player);
        _player.playerInput.DownJump += Judgment;
        _offsetY = Vector3.up * offsetY;
    }
    private void OnDestroy()
    {
        _player.playerInput.DownJump -= Judgment;
    }
    private void Update()
    {
        DuringClimb();
    }

    private void Judgment()
    {
        if (_isClimb)
        {
            EndAction();
            return;
        }

        if (!_player.parkouring)
        {
            Vector3 rayCenter = Vector3.zero;
            Vector3 dir = Vector3.zero;
            if (_player.playerMovement.LockedY == false && CanHangPipe(transform.position, ref rayCenter, ref dir))
            {
                _player.transform.DOMove(rayCenter + Core.RotateVector(dir, _hangOffset), _hangDuratdion);
                _player.transform.DORotateQuaternion(Quaternion.LookRotation(dir) * Quaternion.Euler(-8.38f, 0f, 0f), _hangDuratdion).OnComplete(() =>
                {
                    _isClimb = true;
                });
                StartAction();
            }
        }
    }

    private bool CanHangPipe(Vector3 center, ref Vector3 rayCenter, ref Vector3 dir)
    {
        if(Physics.Raycast(new Ray(center, transform.forward), out RaycastHit hit, hangDistance, _climbLayer) &&
           Physics.Raycast(new Ray(center + _offsetY, transform.forward), out RaycastHit hit2, hangDistance, _climbLayer))
        {
            if (!hit.collider.CompareTag("Pipe") || !hit2.collider.CompareTag("Pipe")) return false;

            float y = (hit.point.y + hit2.point.y) * 0.5f;
            Vector3 pipePos = hit.transform.parent.position;
            rayCenter = new Vector3(pipePos.x, y, pipePos.z);

            dir = -hit2.normal;
            return true;
        }
        return false;
    }

    private void DuringClimb()
    {
        if (!_isClimb) return;
        if (!_movable) return;

        Vector2 dir = _player.playerInput.Movement;
        Vector3 rayCenter = Vector3.zero;
        Vector3 pipeDir = Vector3.zero;

        bool upable = dir.y > 0.3f && Physics.Raycast(new Ray(transform.position + _offsetY + Vector3.up * 0.1f, transform.forward), 0.6f, _climbLayer) &&
                                      !Physics.Raycast(new Ray(transform.position + _offsetY, transform.up), 0.1f, _climbLayer);

        bool downable = dir.y < -0.3f && Physics.Raycast(new Ray(transform.position - Vector3.up * 0.1f, transform.forward), 0.6f, _climbLayer) &&
                                      !Physics.Raycast(new Ray(transform.position, -transform.up), 0.1f, _climbLayer);


        if (upable || downable)
        {
            _player.playerAnim.anim.SetFloat("PipeMoveSpeed", dir.y * 1.1f);
            _player.transform.Translate(Vector3.up * dir.y * speed * Time.deltaTime, Space.World);
        }
        else
        {
            _player.playerAnim.anim.SetFloat("PipeMoveSpeed", 0f);
        }
        if (dir.x > 0.5f )
        {
            if (!Physics.Raycast(new Ray(transform.position, transform.right), sideMoveDis, _climbLayer) &&
                !Physics.Raycast(new Ray(transform.position + _offsetY, transform.right), sideMoveDis, _climbLayer) &&
                CanHangPipe(transform.position + transform.right * sideMoveDis, ref rayCenter, ref pipeDir))
            {
                _movable = false;
                _player.playerAnim.SetTrigger("PipeRight");
                _player.transform.DOMove(new Vector3(rayCenter.x, transform.position.y, rayCenter.z) + Core.RotateVector(pipeDir, _hangOffset), _duration).OnComplete(() => _movable = true);
            }

        }
        else if(dir.x < -0.5f)
        {
            if (!Physics.Raycast(new Ray(transform.position, -transform.right), sideMoveDis, _climbLayer) &&
                !Physics.Raycast(new Ray(transform.position + _offsetY, -transform.right), sideMoveDis, _climbLayer) && 
                CanHangPipe(transform.position + -transform.right * sideMoveDis, ref rayCenter, ref pipeDir))
            {
                _movable = false;
                _player.playerAnim.SetTrigger("PipeLeft");
                _player.transform.DOMove(new Vector3(rayCenter.x, transform.position.y, rayCenter.z) + Core.RotateVector(pipeDir, _hangOffset), _duration).OnComplete(() => _movable = true);
            }
        }

    }

    public override void StartAction()
    {
        base.StartAction();
        _player.playerAnim.SetTrigger("Pipe");
        _player.playerMovement.LockY(true);
        _movable = true;
    }
    public override void EndAction()
    {
        if (!_isClimb) return;

        base.EndAction();
        _player.playerAnim.SetTrigger("Falling");
        _player.playerMovement.UnlockY(true);
        _isClimb = false;
    }
}

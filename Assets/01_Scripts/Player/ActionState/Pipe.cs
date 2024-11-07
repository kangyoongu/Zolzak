using DG.Tweening;
using System;
using UnityEngine;

public class Pipe : ActionState
{
    public float sphereRadius = 0.3f; // SphereCast의 반지름
    public float maxDistance = 5.0f;  // SphereCast의 최대 거리
    public float offsetY = 2f;       // 좌표 오프셋 값
    public float offsetX = 0.5f;       // 좌표 오프셋 값
    public float tolerance = 0.1f;
    public float xWeight = 1f;
    public float yWeight = 1f;
    public float moveDuration = 1f;
    public Vector3 beforeUpOffset = Vector3.zero;
    [SerializeField] Vector3 _offset;
    [SerializeField] float _duration;
    [SerializeField] LayerMask _climbLayer;
    bool _isClimb = false;
    bool _movable = true;
    bool _upable = false;
    float _climbCooltime = 1f;
    Vector3 _upPos = Vector3.zero;
    public override void Init(Player player)
    {
        base.Init(player);
        _player.playerInput.DownJump += Judgment;
    }

    private void Update()
    {
        ClimbingFunc();
    }

    private void Judgment()
    {
        if (_isClimb)
        {
            EndAction();
            return;
        }

        if (_player.playerMovement.grounded == false && _player.playerInput.Jumping && !_player.parkouring && _climbCooltime < 0f)
        {
            if (CheckIfPointsAreOnSamePlane(out Vector3 rayCenter, out Vector3 dir))
            {
                _player.transform.DOMove(rayCenter + transform.TransformDirection(_offset), _duration);
                _player.transform.DORotateQuaternion(Quaternion.LookRotation(dir), _duration).OnComplete(() =>
                {
                    _isClimb = true;
                });
                _player.playerParkour.StartAction("Climb");
            }
        }
    }

    private void ClimbingFunc()
    {
        _climbCooltime -= Time.deltaTime;
        if (!_isClimb || !_movable) return;

        Vector2 dir = _player.playerInput.Movement;

        if (_upable && dir.y > 0f)//벽 위로 올라감
        {
            _player.playerAnim.SetTrigger("ClimbUp");
            _player.transform.DOMove(_upPos, moveDuration);
            _upable = false;
            _movable = false;
        }
        else if (dir.sqrMagnitude > 0.5f)
        {
            Vector3 offset = transform.right * dir.x * xWeight;
            offset += transform.up * dir.y * yWeight;
            float moveDis = offset.magnitude;

            if (!(!Physics.SphereCast(new Ray(transform.position, offset.normalized), 0.2f, moveDis, _climbLayer) &&
                !Physics.SphereCast(new Ray(transform.position + Vector3.up * offsetY, offset.normalized), 0.2f, moveDis, _climbLayer))) return;//움직일 곳 선상에 장애물 없는지

            Vector3 origin = transform.position + offset;

            if (Physics.Raycast(new Ray(origin, transform.forward), 0.5f, _climbLayer) &&
                Physics.Raycast(new Ray(origin + Vector3.up * offsetY, transform.forward), 0.5f, _climbLayer))//밟을 곳 있나
            {
                Move(dir, offset, transform.position + offset);
                _upable = false;
            }
            else//아니면 맨 윗부분인가
            {
                origin = transform.position + Vector3.up * (dir.y * yWeight + offsetY);
                if (!(dir.y > 0f && !Physics.Raycast(new Ray(origin, transform.forward), 0.5f, _climbLayer))) return;

                origin += transform.forward * 0.5f;
                if (!Physics.Raycast(new Ray(origin, Vector3.up), 2f, _climbLayer) &&
                    Physics.Raycast(new Ray(origin, Vector3.down), out RaycastHit hit, 1f, _climbLayer))
                {
                    Move(new Vector2(0f, dir.y), offset, new Vector3(transform.position.x, hit.point.y, transform.position.z) + transform.TransformDirection(beforeUpOffset));
                    _upPos = hit.point;
                    _upable = true;
                }
            }
        }
    }

    private void Move(Vector2 dir, Vector3 offset, Vector3 pos)
    {
        _movable = false;
        _player.playerAnim.anim.SetFloat("ClimbX", dir.x);
        _player.playerAnim.anim.SetFloat("ClimbY", dir.y);
        _player.playerAnim.SetTrigger("ClimbMove");
        _player.transform.DOMove(pos, moveDuration);
    }

    public void Moveable()
    {
        _movable = true;
    }
    public override void StartAction()
    {
        base.StartAction();
        _player.playerAnim.anim.SetBool("Climb", true);
        _player.playerMovement.LockY(true);
    }
    public override void EndAction()
    {
        if (!_isClimb) return;

        base.EndAction();
        _upable = false;
        _player.playerAnim.anim.SetBool("Climb", false);
        _player.playerAnim.SetTrigger("Falling");
        _player.playerMovement.UnlockY(true);
        _isClimb = false;
        _climbCooltime = 1f;
    }
    bool CheckIfPointsAreOnSamePlane(out Vector3 rayCenter, out Vector3 dir)
    {
        rayCenter = Vector3.zero;
        dir = Vector3.zero;
        // 기본 포지션과 방향
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;

        // 네 개의 offset 위치 계산
        Vector3[] offsets = new Vector3[]
        {
            origin + right * offsetX,     // 오른쪽 아래
            origin - right * offsetX,     // 왼쪽 아래
            origin + right * offsetX + up * offsetY,     // 오른쪽 위
            origin - right * offsetX + up * offsetY     // 왼쪽 위
        };

        // 네 개의 SphereCast 충돌 지점 저장
        Vector3[] hitPoints = new Vector3[4];

        Vector3 planeNormal = Vector3.zero;
        for (int i = 0; i < offsets.Length; i++)
        {
            if (Physics.SphereCast(offsets[i], sphereRadius, forward, out RaycastHit hit, maxDistance))
            {
                hitPoints[i] = hit.point;
                planeNormal = hit.normal;
            }
            else return false;
        }

        // 첫 번째 점을 기준으로 평면 정의
        float distanceToPlane = Vector3.Dot(planeNormal, hitPoints[0]);

        // 나머지 세 점이 같은 평면에 있는지 확인
        for (int i = 1; i < hitPoints.Length; i++)
        {
            float distance = Mathf.Abs(Vector3.Dot(planeNormal, hitPoints[i]) - distanceToPlane);

            if (distance > tolerance) return false;  // 오차 범위를 벗어나면 false 반환
        }
        rayCenter = (hitPoints[0] + hitPoints[1]) * 0.5f;
        dir = -planeNormal;
        return true; // 네 점이 같은 평면에 있으면 true 반환
    }
}

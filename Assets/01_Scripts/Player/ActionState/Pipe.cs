using DG.Tweening;
using System;
using UnityEngine;

public class Pipe : ActionState
{
    public float sphereRadius = 0.3f; // SphereCast�� ������
    public float maxDistance = 5.0f;  // SphereCast�� �ִ� �Ÿ�
    public float offsetY = 2f;       // ��ǥ ������ ��
    public float offsetX = 0.5f;       // ��ǥ ������ ��
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

        if (_upable && dir.y > 0f)//�� ���� �ö�
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
                !Physics.SphereCast(new Ray(transform.position + Vector3.up * offsetY, offset.normalized), 0.2f, moveDis, _climbLayer))) return;//������ �� ���� ��ֹ� ������

            Vector3 origin = transform.position + offset;

            if (Physics.Raycast(new Ray(origin, transform.forward), 0.5f, _climbLayer) &&
                Physics.Raycast(new Ray(origin + Vector3.up * offsetY, transform.forward), 0.5f, _climbLayer))//���� �� �ֳ�
            {
                Move(dir, offset, transform.position + offset);
                _upable = false;
            }
            else//�ƴϸ� �� ���κ��ΰ�
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
        // �⺻ �����ǰ� ����
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;

        // �� ���� offset ��ġ ���
        Vector3[] offsets = new Vector3[]
        {
            origin + right * offsetX,     // ������ �Ʒ�
            origin - right * offsetX,     // ���� �Ʒ�
            origin + right * offsetX + up * offsetY,     // ������ ��
            origin - right * offsetX + up * offsetY     // ���� ��
        };

        // �� ���� SphereCast �浹 ���� ����
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

        // ù ��° ���� �������� ��� ����
        float distanceToPlane = Vector3.Dot(planeNormal, hitPoints[0]);

        // ������ �� ���� ���� ��鿡 �ִ��� Ȯ��
        for (int i = 1; i < hitPoints.Length; i++)
        {
            float distance = Mathf.Abs(Vector3.Dot(planeNormal, hitPoints[i]) - distanceToPlane);

            if (distance > tolerance) return false;  // ���� ������ ����� false ��ȯ
        }
        rayCenter = (hitPoints[0] + hitPoints[1]) * 0.5f;
        dir = -planeNormal;
        return true; // �� ���� ���� ��鿡 ������ true ��ȯ
    }
}

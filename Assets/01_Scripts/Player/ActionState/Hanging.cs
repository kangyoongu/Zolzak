using DG.Tweening;
using System;
using UnityEngine;

public class Hanging : ActionState
{
    public float hangDistance = 2.0f;  // SphereCast의 최대 거리
    public float force = 10f;
    public float flyForce = 10f;
    public float sideMoveDis = 1f;
    [SerializeField] Vector3 _hangOffset;
    [SerializeField] float _duration;
    [SerializeField] LayerMask _hangLayer;
    bool _isHanging = false;
    bool _flying = false;
    HingeJoint currentJoint;
    public override void Init(Player player)
    {
        base.Init(player);
        _player.playerInput.DownJump += Judgment;
        _player.OnPlayerCollisionEnter += CollisionEnter;
    }
    private void OnDestroy()
    {
        _player.playerInput.DownJump -= Judgment;
        _player.OnPlayerCollisionEnter -= CollisionEnter;
    }
    private void CollisionEnter(Collision obj)
    {
        if (_flying && obj.gameObject != currentJoint.gameObject)
        {
            EndAction();
        }
    }

    private void Update()
    {
        DuringHanging();
    }

    private void Judgment()
    {
        if (_isHanging && !_flying)
        {
            Flying();
            return;
        }

        if (!_player.parkouring && !_player.playerMovement.grounded)
        {
            if (_player.playerMovement.LockedY == false && 
                Physics.SphereCast(new Ray(transform.position + transform.forward, Vector3.up), 1f, out RaycastHit hit, 2f, _hangLayer))
            {
                if (hit.collider.CompareTag("Hanging"))
                {
                    Vector3 targetPos = hit.transform.InverseTransformPoint(hit.point);
                    targetPos.y = 0f;
                    targetPos.z = 0f;
                    Vector3 targetRotate = -hit.transform.forward;

                    Vector3 directionToTarget = (transform.position - hit.point).normalized;
                    float dot = Vector3.Dot(hit.transform.forward, directionToTarget);

                    if (dot < 0)
                    {
                        targetRotate = hit.transform.forward;
                    }

                    _player.transform.DOMove(hit.transform.TransformPoint(targetPos) + transform.InverseTransformDirection(_hangOffset), _duration);
                    _player.transform.DORotateQuaternion(Quaternion.LookRotation(targetRotate), _duration).OnComplete(() =>
                    {
                        _isHanging = true;
                        currentJoint = hit.collider.GetComponent<HingeJoint>();
                        currentJoint.GetComponent<Collider>().enabled = false;
                        currentJoint.connectedBody = _player.Rigidbody;
                        currentJoint.anchor = new Vector3(targetPos.x, 0f, 0f);
                    });
                    StartAction();
                }
            }
        }
    }

    private void DuringHanging()
    {
        if (!_isHanging) return;

        Vector2 dir = _player.playerInput.Movement;

        if (Mathf.Abs(dir.y) > 0.5f)
        {
            _player.Rigidbody.AddRelativeTorque(Vector3.right * force * -dir.y);
        }
    }

    public override void StartAction()
    {
        base.StartAction();
        _player.playerAnim.SetTrigger("Hanging");
        _player.playerMovement.LockY(true);
        _player.Rigidbody.isKinematic = false;
        _player.Rigidbody.freezeRotation = false;
    }
    public override void EndAction()
    {
        if (!_isHanging) return;

        currentJoint.gameObject.GetComponent<Collider>().enabled = true;

        base.EndAction();
        _flying = false;
        _isHanging = false;
    }
    private void Flying()
    {
        _flying = true;

        Vector3 localAngularVelocity = transform.InverseTransformDirection(_player.Rigidbody.angularVelocity);
        _player.transform.DORotateQuaternion(Quaternion.Euler(0f, transform.eulerAngles.y, 0f), _duration);
        currentJoint.connectedBody = null;
        _player.Rigidbody.freezeRotation = true;

        _player.playerAnim.SetTrigger("Falling");
        _player.playerMovement.grounded = false;
        _player.playerMovement.UnlockY(true);
        _player.Rigidbody.AddForce(transform.forward * -localAngularVelocity.x * flyForce, ForceMode.Impulse);
    }
}

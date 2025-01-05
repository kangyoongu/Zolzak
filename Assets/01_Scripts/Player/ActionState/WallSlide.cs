using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;

public class WallSlide : ActionState
{
    [SerializeField] LayerMask _layerMask;
    public Vector3 wallOffset;
    public float speed;
    bool wallSlide = false;
    Direction direction;
    

    private void FixedUpdate()
    {
        FixVelocity();
    }
    private void LateUpdate()
    {
        SpeedCheck();
    }

    private void SpeedCheck()
    {
        if (!wallSlide) return;

        if(_player.Rigidbody.linearVelocity.sqrMagnitude < 50f)
            EndAction();
    }

    private void FixVelocity()
    {
        if (!wallSlide) return;

        RaycastHit hit;
        if (direction == Direction.Left)
        {
            if (WallCheckRaycast(-transform.right, out hit))
            {
                Vector3 normal = hit.normal;
                normal.y = 0;
                _player.transform.rotation =
                    Quaternion.Lerp(_player.transform.rotation, Quaternion.Euler(0f, -90f, 0f) * Quaternion.LookRotation(hit.normal), 4f * Time.fixedDeltaTime);
                _player.Rigidbody.linearVelocity = (Quaternion.Euler(0f, -90f, 0f) * hit.normal - transform.right * 0.1f) * speed;
                return;
            }
        }
        else
        {
            if (WallCheckRaycast(transform.right, out hit))
            {
                Vector3 normal = hit.normal;
                normal.y = 0;
                _player.transform.rotation = 
                    Quaternion.Lerp(_player.transform.rotation, Quaternion.Euler(0f, 90f, 0f) * Quaternion.LookRotation(hit.normal), 4f * Time.fixedDeltaTime);
                _player.Rigidbody.linearVelocity = (Quaternion.Euler(0f, 90f, 0f) * hit.normal + transform.right * 0.1f) * speed;
                return;
            }
        }

        EndAction();
    }

    public override void Init(Player player)
    {
        base.Init(player);
        _player.playerInput.DownJump += StartAction;
        _player.playerInput.UpJump += EndAction;
    }
    private void OnDestroy()
    {
        _player.playerInput.DownJump -= StartAction;
        _player.playerInput.UpJump -= EndAction;
    }
    public override void StartAction()
    {
        StartCoroutine(FrameLast());
    }
    public override void EndAction()
    {
        if (!wallSlide) return;
        base.EndAction();
        wallSlide = false;
        _player.Rigidbody.useGravity = true;
        _player.playerAnim.anim.SetBool("RightWall", false);
        _player.playerAnim.anim.SetBool("LeftWall", false);
        _player.playerMovement.UnlockY(true);
    }
    IEnumerator FrameLast()
    {
        yield return new WaitForEndOfFrame();
        if (wallSlide) yield break;
        if (_player.parkouring) yield break;

        AnimatorStateInfo stateInfo = _player.playerAnim.anim.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("Falling Idle")) yield break;
        RaycastHit hit;
        if (WallCheckRaycast(transform.right, out hit))
        {
            Vector3 normal = hit.normal;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(_player.Rigidbody.linearVelocity, normal);
            projectedVelocity = transform.InverseTransformDirection(projectedVelocity);
            if (projectedVelocity.z > 4f)
            {
                StartSet(normal);
                _player.playerAnim.anim.SetBool("RightWall", true);
                direction = Direction.Right;
            }
        }
        else if(WallCheckRaycast(-transform.right, out hit))
        {
            Vector3 normal = hit.normal;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(_player.Rigidbody.linearVelocity, normal);
            projectedVelocity = transform.InverseTransformDirection(projectedVelocity);

            if (projectedVelocity.z > 4f)
            {
                StartSet(normal);
                _player.playerAnim.anim.SetBool("LeftWall", true);
                direction = Direction.Left;
            }
        }

    }

    private void StartSet(Vector3 normal)
    {
        base.StartAction();
        normal.y = 0;
        _player.Rigidbody.isKinematic = false;
        _player.Rigidbody.useGravity = false;
        wallSlide = true;
        _player.playerMovement.LockY(true);
    }

    private bool WallCheckRaycast(Vector3 dir, out RaycastHit hit)
    {
        return Physics.SphereCast(new Ray(transform.position, dir), 0.3f, out hit, 1.3f, _layerMask) &&
               Physics.SphereCast(new Ray(transform.position + new Vector3(0f, 1.8f, 0f), dir), 0.3f, 1.3f, _layerMask);
    }
}

public enum Direction : short
{
    Left = 0,
    Right
}
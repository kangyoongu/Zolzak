using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _camSpeed = 5f;
    [SerializeField] private float _jumpPower = 200f;
    [SerializeField] private float _minDamageHeight = 10f;
    [SerializeField] private float _maxDamageHeight = 20f;
    [SerializeField] private float _minDamage = 0.05f;
    [SerializeField] private float _maxDamage = 1f;
    [SerializeField] private float _rollLimite = 0.5f;

    [SerializeField] private Transform _camTrm;

    public LayerMask groundLayer;
    public bool grounded = true;

    private Rigidbody _rb;
    private Player _player;

    float downShiftTimer = 0f;
    float _pitch = 0f;
    float _yaw = 0f;


    Vector2Int _beforeLock = Vector2Int.zero;
    Vector3 _direction = Vector3.zero;
    Vector3 _fallPoint = Vector3.zero;
    int _triggerCnt = 0;

    public bool LockedY { get => _lockY; private set => Debug.Log("Dont try"); }
    bool _lockY = false;
    bool _lockX = false;
    bool _rotateHead = false;
    [HideInInspector] public bool movable = true;
    #region UNITY_EVENTS
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
        _yaw = transform.eulerAngles.y;
    }
    private void OnEnable()
    {
        _player.playerInput.UpShift += PlayerInput_UpShift;
        _player.playerInput.OnAim += Aim;
    }
    private void Update()
    {
        FallingCheck();
        if(movable)
            Move(_player.playerInput.Movement);
    }
    private void OnDisable()
    {
        _player.playerInput.UpShift -= PlayerInput_UpShift;
        _player.playerInput.OnAim -= Aim;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lemon")) return;

        if(_triggerCnt == 0) {

            if (_fallPoint.y - _player.transform.position.y > _minDamageHeight && downShiftTimer <= _rollLimite && downShiftTimer > 0f)
                _player.playerAnim.anim.SetInteger("Landing", 2);
            else
            {
                _player.playerAnim.anim.SetInteger("Landing", 1);
                if (_fallPoint.y - _player.transform.position.y > _minDamageHeight)
                    _player.GetDamage(Core.Remap(_fallPoint.y - _player.transform.position.y, _minDamageHeight, _maxDamageHeight, _minDamage, _maxDamage));
            }

            _fallPoint = Vector3.zero;
        }
        _triggerCnt++;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Lemon")) return;

        if (!grounded)
        {
            grounded = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Lemon")) return;

        _triggerCnt--;
        if (_triggerCnt == 0 && !_player.parkouring)
        {
            _player.playerAnim.anim.SetInteger("Landing", 0);
            _fallPoint = _player.transform.position;
            _player.playerAnim.SetTrigger("Falling");
            grounded = false;
        }
    }
    #endregion

    private void PlayerInput_UpShift()
    {
        downShiftTimer = 0f;
    }

    void Aim(Vector2 pos)
    {
        if (!_lockY || _rotateHead == true)
        {
            if(Cursor.visible == false)
                _yaw += _camSpeed * 0.1f * pos.x;
        }

        if (!_lockX && Cursor.visible == false)
        {
            _pitch += _camSpeed * 0.1f * pos.y;

            _pitch = Mathf.Clamp(_pitch, -60f, 80f);
        }

        if(!_lockY)
            transform.localEulerAngles = new Vector3(0, _yaw, 0);
        if (_rotateHead)
        {
            _yaw = Mathf.Clamp(_yaw, -70f, 70f);
            _camTrm.localEulerAngles = new Vector3(-_pitch, _yaw, 3.5f);
        }
        else if (!_lockY)
            _camTrm.localEulerAngles = new Vector3(-_pitch, 1.09f, 3.5f);
    }

    public void LockY(bool rHead = false)
    {
        _lockY = true;
        if (rHead)
        {
            _yaw = 1.09f;
            _rotateHead = true;
        }
        else
        {
            _rotateHead = false;
        }
    }
    public void UnlockY(bool rHead = false)
    {
        if (rHead)
        {
            DOTween.To(() => _yaw, x => _yaw = x, transform.localEulerAngles.y, 0.5f).OnUpdate(() =>
            {
                _camTrm.localEulerAngles = new Vector3(-_pitch, _yaw, 3.5f);
            });
            _camTrm.DOLocalRotate(new Vector3(_camTrm.localEulerAngles.x, 1.09f, 3.5f), 0.5f).OnComplete(() =>
            {
                _lockY = false;
            });
            _rotateHead = false;
        }
        else
        {
            _yaw = transform.localEulerAngles.y;
            _lockY = false;
        }
    }
    public void LockX()
    {
        _lockX = true;
    }
    public void UnlockX()
    {
        if (_lockX && _lockY && !_rotateHead) return;
        _lockX = false;
    }
    public void Lock()
    {
        _beforeLock.x = _lockX ? 1 : 0;
        _beforeLock.y = _lockY ? 1 : 0;
        _beforeLock.y = _rotateHead ? 2 : _beforeLock.y;
        LockX();
        LockY();
    }
    public void Unlock()
    {
        UnlockY();
        UnlockX();
    }


    private void FallingCheck()
    {
        if (_triggerCnt == 0 && grounded && !_player.parkouring)
        {
            _player.playerAnim.anim.SetInteger("Landing", 0);
            _fallPoint = _player.transform.position;
            _player.playerAnim.SetTrigger("Falling");
            grounded = false;
        }
        if(grounded == false && _player.playerInput.Shift)
        {
            downShiftTimer += Time.deltaTime;
        }
    }

    void Move(Vector2 input)
    {
        int weight = 1;

        if (_player.playerInput.Shift) weight *= 2;

        Vector3 velocity = _rb.linearVelocity;

        Vector3 localMovement = new Vector3(input.x * weight, 0, input.y * weight);
        _direction = Vector3.Lerp(_direction, localMovement, 7f * Time.deltaTime);
        _player.playerAnim.SetDirection(_direction);

        if (_player.parkouring == false)
        {
            Vector3 worldMovement = transform.TransformDirection(_direction);

            velocity.x = worldMovement.x * _moveSpeed;
            velocity.z = worldMovement.z * _moveSpeed;

            _rb.linearVelocity = velocity;
        }
    }

    public void Jump()
    {
        if (grounded && _rb.isKinematic == false && !_player.parkouring)
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _jumpPower, _rb.linearVelocity.z);
        }
    }
}

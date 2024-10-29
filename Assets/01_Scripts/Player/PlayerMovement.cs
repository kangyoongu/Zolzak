using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _camSpeed = 5f;
    [SerializeField] private float _jumpPower = 200f;
    [SerializeField] private float _rollHeight = 10f;
    [SerializeField] private Transform _camTrm;

    private Transform _camParent;
    private Player _player;
    private Rigidbody _rb;

    float _pitch = 0f;
    float _yaw = 0f;

    public LayerMask groundLayer;
    public bool grounded = true;

    Vector3 _direction = Vector3.zero;
    int _triggerCnt = 0;
    Vector3 _fallPoint = Vector3.zero;

    bool _lockY = false;
    bool _rotateHead = false;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
        _camParent = _camTrm.parent;
        _player.playerInput.OnAim += Aim;
    }
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Aim(Vector2 pos)
    {
        _yaw += _camSpeed * 0.1f * pos.x; 
        _pitch += _camSpeed * 0.1f * pos.y; 

        _pitch = Mathf.Clamp(_pitch, -60f, 80f);

        if(!_lockY)
            transform.localEulerAngles = new Vector3(0, _yaw, 0);
        if (_rotateHead)
        {
            _yaw = Mathf.Clamp(_yaw, -70f, 70f);
            _camTrm.localEulerAngles = new Vector3(-_pitch, _yaw, 3.5f);
        }
        else
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
    }
    public void UnlockY(bool rHead = false)
    {
        _yaw = transform.localEulerAngles.y;
        if (rHead)
        {
            _camTrm.DOLocalRotate(new Vector3(_camTrm.localEulerAngles.x, 1.09f, 3.5f), 0.5f).OnComplete(() =>
            {
                _lockY = false;
            });
            _rotateHead = false;
        }
        else
            _lockY = false;
    }
    private void Update()
    {
        FallingCheck();
        Move(_player.playerInput.Movement);
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
    }

    void Move(Vector2 input)
    {
        if (_player.parkouring == false)
        {
            int weight = 1;
            if (_player.playerInput.Shift)
                weight *= 2;

            Vector3 velocity = _rb.linearVelocity;

            Vector3 localMovement = new Vector3(input.x * weight, 0, input.y * weight);
            _direction = Vector3.Lerp(_direction, localMovement, 7f * Time.deltaTime);
            _player.playerAnim.SetDirection(_direction);

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
    private void OnTriggerEnter(Collider other)
    {
        if(_triggerCnt == 0) {
            if (_fallPoint.y - _player.transform.position.y > _rollHeight)
                _player.playerAnim.anim.SetInteger("Landing", 2);
            else
                _player.playerAnim.anim.SetInteger("Landing", 1);

            _fallPoint = Vector3.zero;
        }
        _triggerCnt++;
    }
    private void OnTriggerStay(Collider other)
    {
        if (!grounded)
        {
            grounded = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        _triggerCnt--;
        if (_triggerCnt == 0 && !_player.parkouring)
        {
            _player.playerAnim.anim.SetInteger("Landing", 0);
            _fallPoint = _player.transform.position;
            _player.playerAnim.SetTrigger("Falling");
            grounded = false;
        }
    }
}

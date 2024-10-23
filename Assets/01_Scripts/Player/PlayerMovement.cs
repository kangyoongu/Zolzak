using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _camSpeed = 5f;
    [SerializeField] private float _jumpPower = 200f;
    [SerializeField] private Transform _camTrm;
    [SerializeField] private Transform _spine;
    private Transform _camParent;
    private Player _player;

    float _pitch = 0f;
    float _yaw = 0f;

    public LayerMask groundLayer;

    private Rigidbody _rb;
    Vector3 direction = Vector3.zero;
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

        _pitch = Mathf.Clamp(_pitch, -70f, 90f);

        transform.localEulerAngles = new Vector3(0, _yaw, 0);
        _camTrm.eulerAngles = new Vector3(_camParent.eulerAngles.x + -_pitch, _camTrm.eulerAngles.y, 0f);
    }
    private void Update()
    {
        Move(_player.playerInput.Movement);
    }

    void Move(Vector2 input)
    {
        Ray ray = new Ray(_spine.position, -_spine.up);
        if (Physics.Raycast(ray, 1.1f, groundLayer) && _player.parkouring == false)
        {
            int weight = 1;
            if (_player.playerInput.Shift)
                weight *= 2;

            Vector3 velocity = _rb.linearVelocity;

            Vector3 localMovement = new Vector3(input.x * weight, 0, input.y * weight);
            direction = Vector3.Lerp(direction, localMovement, 7f * Time.deltaTime);
            _player.playerAnim.SetDirection(direction);

            Vector3 worldMovement = transform.TransformDirection(direction);

            velocity.x = worldMovement.x * _moveSpeed;
            velocity.z = worldMovement.z * _moveSpeed;

            _rb.linearVelocity = velocity;
        }
    }

    public void Jump()
    {
        Ray ray = new Ray(_spine.position, -_spine.up);
        if (Physics.Raycast(ray, 1.1f, groundLayer))
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _jumpPower, _rb.linearVelocity.z);
        }
    }
}

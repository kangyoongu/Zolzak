using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;    // �̵� �ӵ�
    public float camSpeed = 5f;
    public float jumpPower = 200f;
    public LayerMask groundLayer;
    float _pitch = 0f;
    float _yaw = 0f;
    public PlayerInput _inputCompo;
    private Rigidbody _rb;           // Rigidbody ������Ʈ
    Transform _camTrm;
    private void Awake()
    {
        _camTrm = transform.Find("Main Camera");
        _rb = GetComponent<Rigidbody>();
        _inputCompo.OnAim += Aim;
        _inputCompo.OnJump += Jump;
    }
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Aim(Vector2 pos)
    {
        _yaw += camSpeed * pos.x; // ���콺X���� ���������� ���� ����
        _pitch += camSpeed * pos.y; // ���콺y���� ���������� ���� ����

        // Mathf.Clamp(x, �ּҰ�, �ִ�) - x���� �ּ�,�ִ밪 ���̿����� ���ϰ� ����
        _pitch = Mathf.Clamp(_pitch, -90f, 90f); // pitch���� ����������

        _camTrm.eulerAngles = new Vector3(-_pitch, transform.eulerAngles.y, transform.eulerAngles.z); // �ޱ۰��� �������� ���� �־���
        transform.eulerAngles = new Vector3(0, _yaw, 0);
    }
    private void Update()
    {
        Move(_inputCompo.Movement);
    }

    void Move(Vector2 input)
    {
        // ���� Rigidbody�� velocity ���� ������
        Vector3 velocity = _rb.linearVelocity;

        // ���� ��ü�� ���� ��ǥ�迡�� XZ ��鿡�� �������� ����
        Vector3 localMovement = new Vector3(input.x * moveSpeed, 0, input.y * moveSpeed);

        // ���� ��ǥ�迡�� ���� ��ǥ��� ��ȯ
        Vector3 worldMovement = transform.TransformDirection(localMovement);

        // Y���� ���� ��ǥ��� �����ϸ鼭 ������ ����
        velocity.x = worldMovement.x;
        velocity.z = worldMovement.z;

        // Rigidbody�� velocity�� ���ο� ���� ����
        _rb.linearVelocity = velocity;
    }

    void Jump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, 1.05f, groundLayer))
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, jumpPower, _rb.linearVelocity.z);
        }
    }
}

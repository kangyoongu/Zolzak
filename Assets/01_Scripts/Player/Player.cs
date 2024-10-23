using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] private Rigidbody _rigid;
    public Rigidbody Rigidbody { get => _rigid; }
    [HideInInspector] public PlayerAnimation playerAnim;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerParkour playerParkour;
    public PlayerInput playerInput;
    CapsuleCollider _capsuleCollider;
    [HideInInspector] public bool parkouring = false;
    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();

        playerAnim = GetComponent<PlayerAnimation>();
        playerMovement = GetComponent<PlayerMovement>();
        playerParkour = GetComponent<PlayerParkour>();
        _capsuleCollider = transform.Find("Collider").GetComponent<CapsuleCollider>();
    }
    public void StartPhysics()
    {
        _capsuleCollider.enabled = false;
        _rigid.isKinematic = true;
        parkouring = true;
    }
    public void EndPhysics()
    {
        _capsuleCollider.enabled = true;
        _rigid.isKinematic = false;
        parkouring = false;
    }
}

using System;
using System.Collections.Generic;
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

    [SerializeField] List<Collider> bodyCollider;

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
        OnBodyCollider();
        _rigid.isKinematic = true;
        parkouring = true;
    }
    public void EndPhysics()
    {
        _capsuleCollider.enabled = true;
        _rigid.isKinematic = false;
        OffBodyCollider();
        StartCoroutine(Core.DelayFrame(() => parkouring = false));
    }

    public void OffBodyCollider()
    {
        foreach(Collider collider in bodyCollider)
        {
            collider.enabled = false;
        }
    }
    public void OnBodyCollider()
    {
        foreach (Collider collider in bodyCollider)
        {
            collider.enabled = true;
        }
    }

    public void GetDamage(float damage)
    {

    }
}

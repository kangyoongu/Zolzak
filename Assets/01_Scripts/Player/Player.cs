using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [HideInInspector] private Rigidbody _rigid;

    public Rigidbody Rigidbody { get => _rigid; }
    [HideInInspector] public PlayerAnimation playerAnim;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerParkour playerParkour;
    public UnityEvent<float> OnDamage;
    int lemonCnt = 5;
    float _hp = 1f;
    public float Hp
    {
        get
        {
            return _hp;
        }
        private set
        {
            _hp = value;
            OnDamage?.Invoke(_hp);
        }
    }

    public void EatLemon()
    {
        lemonCnt--;
        if (lemonCnt <= 0)
            GameManager.Instance.Clear();
    }

    public PlayerInput playerInput;
    CapsuleCollider _capsuleCollider;


    [HideInInspector] public bool parkouring = false;

    [SerializeField] List<Collider> bodyCollider;

    public Action<Collision> OnPlayerCollisionEnter;
    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();

        playerAnim = GetComponent<PlayerAnimation>();
        playerMovement = GetComponent<PlayerMovement>();
        playerParkour = GetComponent<PlayerParkour>();
        _capsuleCollider = transform.Find("Collider").GetComponent<CapsuleCollider>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Pause()
    {
        playerMovement.Lock();
        playerAnim.anim.speed = 0;
        playerMovement.movable = false;
    }
    public void Unpause()
    {
        playerMovement.Unlock();
        playerAnim.anim.speed = 1;
        playerMovement.movable = true;
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
        if (playerMovement.movable)
        {
            Hp -= damage;
            if(Hp <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Pause();
        StartCoroutine(Respawn());
    }
    IEnumerator Respawn()
    {
        GameManager.Instance.SceneChangeOn(GameManager.Instance.darkSphere, GameManager.Instance.darkSphereMat);
        yield return new WaitForSeconds(3f);
        transform.position = GameManager.Instance.spawnPoint.position;
        Unpause();
        GameManager.Instance.SceneChangeOff(GameManager.Instance.darkSphere, GameManager.Instance.darkSphereMat);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnPlayerCollisionEnter?.Invoke(collision);
    }
}

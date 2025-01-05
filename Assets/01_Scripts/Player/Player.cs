using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [HideInInspector] private Rigidbody _rigid;

    public Rigidbody Rigidbody { get => _rigid; }
    [HideInInspector] public PlayerAnimation playerAnim;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerParkour playerParkour;
    public UnityEvent<float> OnDamage;
    public UnityEvent<int> OnEatLemon;
    [SerializeField] int _lemonCnt;
    bool eatable = true;
    public int LemonCnt { 
        get => _lemonCnt;
        set
        {
            _lemonCnt = value;
            OnEatLemon?.Invoke(_lemonCnt);
        } 
    }
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


    public PlayerInput playerInput;
    CapsuleCollider _capsuleCollider;


    [HideInInspector] public bool parkouring = false;

    [SerializeField] List<Collider> bodyCollider;

    public Action<Collision> OnPlayerCollisionEnter;
    private void Awake()
    {
        Application.targetFrameRate = 120;
            
        _rigid = GetComponent<Rigidbody>();

        playerAnim = GetComponent<PlayerAnimation>();
        playerMovement = GetComponent<PlayerMovement>();
        playerParkour = GetComponent<PlayerParkour>();
        _capsuleCollider = transform.Find("Collider").GetComponent<CapsuleCollider>();
        if (gameObject.scene.name == "MainScene")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        _lemonCnt = UIManager.Instance.maxLemon;
        LemonCnt = LemonCnt;
    }
    public void EatLemon()
    {
        if (!eatable) return;
        eatable = false;
        LemonCnt--;
        if (LemonCnt <= 0)
            GameManager.Instance.Clear();

        else
            StartCoroutine(Core.DelayTime(() => eatable = true, 0.1f));
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
        GameManager.Instance.diePlayer?.Invoke();
        Pause();
        StartCoroutine(Respawn());
    }
    IEnumerator Respawn()
    {
        GameManager.Instance.SceneChangeOn(GameManager.Instance.darkSphere, GameManager.Instance.darkSphereMat);
        yield return new WaitForSeconds(3f);
        transform.position = GameManager.Instance.spawnPoint.position;
        transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        Hp = 1f;
        DOTween.Kill(transform);
        GameManager.Instance.ResetObjects();
        _lemonCnt = UIManager.Instance.maxLemon;
        Unpause();
        _rigid.linearVelocity = Vector3.zero;
        GameManager.Instance.SceneChangeOff(GameManager.Instance.darkSphere, GameManager.Instance.darkSphereMat);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnPlayerCollisionEnter?.Invoke(collision);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Die"))
        {
            Die();
        }
    }
}

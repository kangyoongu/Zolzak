using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : SingleTon<GameManager>
{
    public UnityEvent LockCursor;
    public UnityEvent UnlockCursor;
    public Player player;
    public Canvas canvas;
    public RenderTexture fullscreenTexture;
    public Transform rightUp;
    public Transform spawnPoint;
    [SerializeField] Camera _renderCam;

    [HideInInspector] public Material sphereMat;
    public Transform sphere;
    [HideInInspector] public Material darkSphereMat;
    public Transform darkSphere;

    public List<GameObject> lemons;
    public List<GameObject> windows;
    public List<GameObject> inWindowObj;
    public Action diePlayer;

    void Start()
    {
        Core.SetCustomCursor(Core.NORMAL);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        sphereMat = sphere.GetComponent<Renderer>().material;
        darkSphereMat = darkSphere.GetComponent<Renderer>().material;
        SceneChangeOff(sphere, sphereMat);
        FitWindowScene();
    }
    public void ResetObjects()
    {
        foreach(GameObject lemon in lemons)
        {
            lemon.SetActive(true);
        }
        foreach (GameObject window in windows)
        {
            window.SetActive(true);
        }
        foreach (GameObject obj in inWindowObj)
        {
            Destroy(obj);
        }
    }
    public void Clear()
    {
        player.Pause();
        SceneChangeOn(sphere, sphereMat);
    }
    public void SceneChangeOn(Transform target, Material mat)
    {
        target.DOScale(Vector3.one * 0.1f, 3f).SetEase(Ease.OutCubic);
        mat.DOFloat(1f, "_Lerp", 3f).SetEase(Ease.OutCubic);
    }
    public void SceneChangeOff(Transform target, Material mat)
    {
        target.DOScale(Vector3.one * 100f, 3f).SetEase(Ease.OutCubic);
        mat.DOFloat(0f, "_Lerp", 3f).SetEase(Ease.OutCubic);
    }
    private void FitWindowScene()
    {
        Vector2 resolution = ScaleResolution(Screen.width, Screen.height);
        fullscreenTexture = new RenderTexture((int)resolution.x, (int)resolution.y, 24);
        fullscreenTexture.Create();

        // 카메라의 타겟 텍스처를 RenderTexture로 설정
        _renderCam.targetTexture = fullscreenTexture;
    }
    public Vector2 ScaleResolution(float x, float y)
    {
        float maxDimension = Mathf.Max(x, y);
        Vector2 resol = new Vector2(x, y);
        return maxDimension <= 1000f ? resol : resol * (1000f / maxDimension);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(Cursor.visible == false)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                UnlockCursor?.Invoke();
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                LockCursor?.Invoke();
            }
        }
    }
}

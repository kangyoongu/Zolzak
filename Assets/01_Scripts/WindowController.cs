using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowController : MonoBehaviour
{
    public Image targetImage;     // 연결할 RawImage
    public GameObject renderCamera;         // RenderTexture를 적용할 카메라
    public float outlineWidth;
    public bool fixRatio;
    public Vector3 offset;
    
    private Camera _camera;
    private FollowerCam _camCompo;
    private Camera _mainCam;
    private RectTransform _rectTransform;
    private Transform _parent;
    Material _imageMat;

    bool _dragging = false;
    float _ratioX;
    float _ratioY;
    float _scaleRatio;
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _parent = transform.parent;

        _mainCam = Camera.main;
        GameObject cam = Instantiate(renderCamera);
        cam.transform.parent = Camera.main.transform;
        cam.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        //_camera = cam.GetComponent<Camera>();
        //_camCompo = cam.GetComponent<FollowerCam>();
        //_camCompo.Init(_mainCam.transform, offset);
    }
    void Start()
    {
        _scaleRatio = 1f / _rectTransform.sizeDelta.x;
        _ratioX = _rectTransform.sizeDelta.x / _rectTransform.sizeDelta.y;
        _ratioY = _rectTransform.sizeDelta.y / _rectTransform.sizeDelta.x;

        Open();
        OnChangeWindow();
    }

    public void OnChangeWindow()
    {

    }
    public void Open()
    {
        _parent.DOScale(Vector3.one, 0.2f).SetEase(Ease.Linear);
    }
    public void Close()
    {
        _parent.DOScale(Vector3.zero, 0.2f).SetEase(Ease.Linear);
        _dragging = false;
    }
}

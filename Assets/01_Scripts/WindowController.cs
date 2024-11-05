using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerMoveHandler, IPointerExitHandler
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

    Vector2 _clickPoint;
    Vector2 _clickPos;
    Vector2 _clickSize;
    DragDirection _clickDirection;

    bool _dragging = false;
    float _ratioX;
    float _ratioY;
    float _scaleRatio;
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _parent = transform.parent;

        _mainCam = Camera.main;
        //GameObject cam = Instantiate(renderCamera);

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
        _imageMat = Instantiate(targetImage.material);
        _imageMat.SetTexture("_WindowTex", GameManager.Instance.fullscreenTexture);
        targetImage.material = _imageMat;
        OnChangeWindow();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 localPoint = eventData.position - (Vector2)transform.position;
        // 이미지의 크기 가져오기
        Vector2 imageSize = _rectTransform.rect.size - (_rectTransform.rect.size * 0.5f);
        // 외곽선 여부 확인
        if (IsPointInOutline(localPoint, imageSize, out _clickDirection))
        {
            _clickPoint = eventData.position;
            _dragging = true;
            _clickPos = transform.position;
            _clickSize = _rectTransform.rect.size;
        }
    }

    private bool IsPointInOutline(Vector2 localPoint, Vector2 imageSize)
    {
        Vector2 min = new Vector2(outlineWidth - imageSize.x, outlineWidth - imageSize.y);
        Vector2 max = new Vector2(imageSize.x - outlineWidth, imageSize.y - outlineWidth);

        return localPoint.x < min.x || localPoint.x > max.x || localPoint.y < min.y || localPoint.y > max.y;
    }
    private bool IsPointInOutline(Vector2 localPoint, Vector2 imageSize, out DragDirection dir)
    {
        dir = DragDirection.None;
        Vector2 min = new Vector2(outlineWidth - imageSize.x, outlineWidth - imageSize.y);
        Vector2 max = new Vector2(imageSize.x - outlineWidth, imageSize.y - outlineWidth);

        if (localPoint.x < min.x && localPoint.y < min.y)
            dir = DragDirection.DL;
        else if (localPoint.x < min.x && localPoint.y > max.y)
            dir = DragDirection.UL;
        else if (localPoint.x > max.x && localPoint.y < min.y)
            dir = DragDirection.DR;
        else if (localPoint.x > max.x && localPoint.y > max.y)
            dir = DragDirection.UR;

        else if (localPoint.x < min.x)
            dir = DragDirection.L;
        else if (localPoint.x > max.x)
            dir = DragDirection.R;
        else if (localPoint.y < min.y)
            dir = DragDirection.D;
        else if (localPoint.y > max.y)
            dir = DragDirection.U;

        return dir != DragDirection.None;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragging)
        {
            if (_clickDirection == DragDirection.None) return;

            int[] sign = Core.ShortToSignArray((short)_clickDirection);
            if ((short)_clickDirection > 3)
            {
                if (sign[1] == 1)
                {
                    DragMoveY(eventData, sign[0], 50f);
                    if (fixRatio)
                        DragMoveY(eventData, sign[0], 50f * _ratioX, false, _ratioX, false);
                }
                else
                {
                    DragMoveX(eventData, sign[0], 50f * _ratioX);
                    if (fixRatio)
                        DragMoveX(eventData, sign[0], 50f, false, _ratioY, false);
                }
            }
            else//대각선
            {
                if (fixRatio) {
                    DragMoveAvg(eventData, sign[0], sign[1], 50f);
                }
                else
                {
                    DragMoveY(eventData, sign[1], 50f);
                    DragMoveX(eventData, sign[0], 50f);
                }
            }
            OnChangeWindow();
        }
    }
    private void DragMoveX(PointerEventData eventData, int sign, float minSize, bool changeX = true, float weight = 1f, bool move = true)
    {
        float offset = _clickPoint.x - eventData.position.x;
        offset *= weight;
        float signOffset = sign * offset;

        float sizeDelta = changeX ? _clickSize.x : _clickSize.y;
        if (sizeDelta < minSize - signOffset)
        {
            signOffset = minSize - sizeDelta;
            offset = sign * signOffset;
        }

        if (changeX)
        {
            if(move)
                transform.position = new Vector3(_clickPos.x + (offset * -0.5f), transform.position.y, transform.position.z);

            _rectTransform.sizeDelta = new Vector2(sizeDelta + signOffset, _rectTransform.sizeDelta.y);
        }
        else
        {
            if (move)
                transform.position = new Vector3(transform.position.x, _clickPos.y + (offset * -0.5f), transform.position.z);

            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, sizeDelta + signOffset);
        }
    }

    private void DragMoveY(PointerEventData eventData, int sign, float minSize, bool changeY = true, float weight = 1f, bool move = true)
    {
        float offset = _clickPoint.y - eventData.position.y;
        offset *= weight;
        float signOffset = sign * offset;

        float sizeDelta = changeY ? _clickSize.y : _clickSize.x;
        if (sizeDelta < minSize - signOffset)
        {
            signOffset = minSize - sizeDelta;
            offset = sign * signOffset;
        }

        if (changeY)
        {
            if (move)
                transform.position = new Vector3(transform.position.x, _clickPos.y + (offset * -0.5f), transform.position.z);

            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, sizeDelta + signOffset);
        }
        else
        {
            if (move)
                transform.position = new Vector3(_clickPos.x + (offset * -0.5f), transform.position.y, transform.position.z);

            _rectTransform.sizeDelta = new Vector2(sizeDelta + signOffset, _rectTransform.sizeDelta.y);
        }
    }
    private void DragMoveAvg(PointerEventData eventData, int signX, int signY, float minSize)
    {
        float offset = (_clickPoint.y - eventData.position.y) * _ratioX * signY;
        offset += (_clickPoint.x - eventData.position.x) * signX;
        offset *= 0.25f;

        if (_clickSize.y < minSize - offset)
        {
            offset = 50f - _clickSize.y;
        }
        transform.position = new Vector3(_clickPos.x + (offset * _ratioX * signX * -0.5f), _clickPos.y + (offset * signY * -0.5f), transform.position.z);
        _rectTransform.sizeDelta = new Vector2(_clickSize.x + offset * _ratioX, _clickSize.y + offset);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(_dragging)
            _dragging = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        Vector2 localPoint = eventData.position - (Vector2)transform.position;
        // 이미지의 크기 가져오기
        Vector2 imageSize = _rectTransform.rect.size - (_rectTransform.rect.size * 0.5f);
        // 외곽선 여부 확인
        if (IsPointInOutline(localPoint, imageSize))
        {
            Core.SetCursorNS();
        }
        else
            Core.ResetToDefaultCursor();
    }
    public void OnChangeWindow()
    {
        Vector2 pos = targetImage.transform.position;
        Vector2 size = new Vector2(targetImage.rectTransform.rect.width, targetImage.rectTransform.rect.height);
        pos -= size * 0.5f;
        float x = Mathf.Lerp(0, 1, pos.x / Screen.width);
        float y = Mathf.Lerp(0, 1, pos.y / Screen.height);

        _imageMat.SetVector("_Pos", new Vector2(x, y));

        // x와 y 비율 계산
        x = size.x / Screen.width;
        y = size.y / Screen.height;

        _imageMat.SetVector("_Size", new Vector2(x, y));
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Core.ResetToDefaultCursor();
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

public enum DragDirection : byte
{
    UR = 0b000,   
    DR = 0b010,   
    DL = 0b011,   
    UL = 0b001,
    U = 0b110,
    D = 0b111,   
    R = 0b100,
    L = 0b101,   
    None = 0b1111
}
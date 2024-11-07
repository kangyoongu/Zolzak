using UnityEngine;
using UnityEngine.EventSystems;

public class WindowMove : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public RectTransform parentRectTransform; // 이미지 부모의 RectTransform
    private Vector2 _offset; // 드래그 시작 위치와 이미지 중심 간의 거리 저장
    WindowController _windowController;
    private void Start()
    {
        _windowController = transform.parent.GetComponent<WindowController>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _offset = parentRectTransform.position - (Vector3)eventData.position;
    }
    public void OnDrag(PointerEventData eventData)
    {
        parentRectTransform.position = eventData.position + _offset;
        _windowController.ChangeWindow();
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class WindowMove : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public RectTransform parentRectTransform; // �̹��� �θ��� RectTransform
    private Vector2 _offset; // �巡�� ���� ��ġ�� �̹��� �߽� ���� �Ÿ� ����
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

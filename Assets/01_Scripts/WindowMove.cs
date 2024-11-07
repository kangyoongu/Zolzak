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
        Vector2 before = parentRectTransform.position;

        Vector2 targetPos = eventData.position + _offset;
        parentRectTransform.position = targetPos;

        bool outX = _windowController.IsImagePartiallyOutOfScreenX(parentRectTransform, out float valueX);
        bool outY = _windowController.IsImagePartiallyOutOfScreenY(parentRectTransform, out float valueY);

        parentRectTransform.position = new Vector3(outX ? valueX : targetPos.x, outY ? valueY: targetPos.y, parentRectTransform.position.z);

        if((Vector2)parentRectTransform.position != before)
            _windowController.ChangeWindow();
    }
}

using UnityEngine;

public class KeyWindowCam : WindowCam
{
    private Quaternion _beginAngle;
    #region UNITY_EVENTS
    private void Awake()
    {
        _container = transform.GetChild(0);
        _distance = _container.localPosition.z;
        _beginAngle = _container.rotation;

        _rightUp = GameManager.Instance.rightUp;
    }
    private void Update()
    {
        _container.rotation = _beginAngle;
    }
    private void OnDestroy()
    {
        _controller.OnChangeWindow -= OnChangeWindow;
        _controller.OnClose -= Realize;
    }
    #endregion


    protected override void OnChangeWindow(float scale, Vector2 pos)
    {
        float dist = _distance * scale;
        float angleX = Mathf.Lerp(-1f, 1f, pos.x);
        float angleY = Mathf.Lerp(-1f, 1f, pos.y);

        float x = _rightUp.localPosition.x * dist;
        float y = _rightUp.localPosition.y * dist;
        x *= angleX;
        y *= angleY;
        _container.localPosition = new Vector3(x, y, dist);
    }
}

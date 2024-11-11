using UnityEngine;

public class FollowerCam : WindowCam
{
    #region UNITY_EVENTS
    private void Awake()
    {
        _container = transform.GetChild(0);
        _distance = _container.localPosition.z;
        _rightUp = GameManager.Instance.rightUp;
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

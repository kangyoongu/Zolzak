using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FollowerCam : MonoBehaviour
{
    Camera _cam;
    WindowController _controller;
    Vector3 _offset;

    public LayerMask camRender;
    Transform _container;
    float _distance;
    Transform _rightUp;
    private void Awake()
    {
        _container = transform.GetChild(0);
        _distance = _container.localPosition.z;
    }
    public void Init(Transform target, Vector3 offset, WindowController controller, float xWeight, bool onlyWindow = true)
    {
        _offset = offset;
        _controller = controller;
        _controller.OnChangeWindow += OnChangeWindow;
        _controller.OnClose += Realize;
        _cam = GetComponent<Camera>();
        transform.parent = target;
        transform.SetLocalPositionAndRotation(offset, Quaternion.identity);
        _distance *= xWeight;

        if (!onlyWindow)
        {
            _cam.clearFlags = CameraClearFlags.Skybox;
            _cam.allowHDR = true;
            _cam.cullingMask = camRender;
            _cam.GetUniversalAdditionalCameraData().renderPostProcessing = true;
        }
    }
    private void OnDestroy()
    {
        _controller.OnChangeWindow -= OnChangeWindow;
        _controller.OnClose -= Realize;
    }
    private void OnChangeWindow(float scale, Vector2 pos)
    {
        if(!_rightUp)
            _rightUp = transform.parent.Find("RightUp");
        float dist = _distance * scale;
        float angleX = Mathf.Lerp(-1f, 1f, pos.x);
        float angleY = Mathf.Lerp(-1f, 1f, pos.y);

        float x = _rightUp.localPosition.x * dist;
        float y = _rightUp.localPosition.y * dist;
        x *= angleX;
        y *= angleY;
        _container.localPosition = new Vector3(x, y, dist);
    }
    public void Realize()
    {
        while(_container.childCount > 0)
        {
            Transform child = _container.GetChild(0);
            child.parent = null;
            child.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
}

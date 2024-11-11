using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class WindowCam : MonoBehaviour
{
    public LayerMask camRender;
    protected WindowController _controller;
    protected Camera _cam;
    protected Vector3 _offset;

    protected Transform _container;
    protected Transform _rightUp;
    protected float _distance;

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
    protected abstract void OnChangeWindow(float scale, Vector2 pos);

    public void Realize()
    {
        while (_container.childCount > 0)
        {
            Transform child = _container.GetChild(0);
            child.parent = null;
            child.gameObject.layer = LayerMask.NameToLayer("Default");
            if (child.TryGetComponent(out Collider collider))
            {
                if (collider.enabled == false)
                    collider.enabled = true;
            }
        }
        Destroy(_container.gameObject);
    }
}

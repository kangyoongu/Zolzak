using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FollowerCam : MonoBehaviour
{
    private Vector3 _offset;
    Camera cam;
    public LayerMask camRender;
    public void Init(Transform target, Vector3 offset, bool onlyWindow = true)
    {
        _offset = offset;
        cam = GetComponent<Camera>();
        transform.parent = target;
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        if (!onlyWindow)
        {
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.allowHDR = true;
            cam.cullingMask = camRender;
            cam.GetUniversalAdditionalCameraData().renderPostProcessing = true;
        }
    }

}

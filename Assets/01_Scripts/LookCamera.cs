using UnityEngine;

public class LookCamera : MonoBehaviour
{
    Transform _cam;
    private void Awake()
    {
        _cam = Camera.main.transform;
    }
    void LateUpdate()
    {
        transform.rotation = _cam.rotation;
    }
}

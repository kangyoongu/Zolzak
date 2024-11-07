using UnityEngine;

public class CamRangeSetting : MonoBehaviour
{
    public Transform pivot;
    [HideInInspector] public Transform rightUp;
    public float fovX;
    public float fovY;
    void Awake()
    {
        GetCamAngle();
        rightUp = pivot.GetChild(0);
        pivot.localEulerAngles = new Vector3(0f, fovX * 0.5f, 0f);
        rightUp.parent = transform;
        float ratio = 1f / rightUp.localPosition.z;
        float x = rightUp.localPosition.x * ratio;
        rightUp.parent = pivot;
        pivot.localEulerAngles = new Vector3(fovY * 0.5f, 0f, 0f);
        rightUp.parent = transform;
        ratio = 1f / rightUp.localPosition.z;
        rightUp.localPosition = new Vector3(x, rightUp.localPosition.y * -ratio, 1f);
    }
    private void GetCamAngle()
    {
        fovY = Definder.MainCam.fieldOfView;

        // 화면 비율 (가로 / 세로)
        float aspectRatio = Definder.MainCam.aspect;

        // FOVy를 라디안으로 변환
        float fovYRad = Mathf.Deg2Rad * fovY / 2;

        // 정확한 FOVx 계산
        fovX = 2 * Mathf.Rad2Deg * Mathf.Atan(Mathf.Tan(fovYRad) * aspectRatio);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChangeFile : MonoBehaviour
{
    public LayerMask selfLayer;  // 자신이 포함된 레이어만 검출할 레이어 마스크
    bool _clickLogo = false;
    Camera _cam;
    public Player player;
    private void Awake()
    {
        _cam = Camera.main;
    }
    private void OnEnable()
    {
        player.playerInput.LCDown += LClickDown;
        player.playerInput.LCUp += LClickUp;
    }

    private void OnDisable()
    {
        player.playerInput.LCDown -= LClickDown;
        player.playerInput.LCUp -= LClickUp;
    }
    private void LClickUp()
    {
        if (DetectSelf() && _clickLogo)
        {
            SceneManager.LoadScene(1);
        }
        _clickLogo = false;
    }

    private void LClickDown()
    {
        if (DetectSelf())
            _clickLogo = true;
    }

    private bool DetectSelf()
    {
        if (_cam == null) return false;

        // 카메라의 중앙 스크린 좌표에서 월드 방향으로 레이 생성
        Ray ray = _cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        // 레이캐스트 실행
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, selfLayer))
        {
            // 자기 자신이 검출되었는지 확인
            return hitInfo.collider.gameObject == gameObject;
        }

        return false;  // 아무것도 검출되지 않음
    }
}

using System;
using UnityEngine;

public class SpawnWindow : MonoBehaviour
{
    public LayerMask selfLayer;
    public GameObject spawnWindow;
    bool _clickLogo = false;
    Camera _cam;
    GameManager _gameManager;
    private void Awake()
    {
        _cam = Camera.main;
    }
    private void Start()
    {
        if (!_gameManager)
            _gameManager = GameManager.Instance;

        StartCoroutine(Core.DelayFrame(() => _gameManager.windows.Add(gameObject)));
    }
    private void OnEnable()
    {
        if (!_gameManager)
            _gameManager = GameManager.Instance;

        StartCoroutine(Core.DelayFrame(() => {
            _gameManager.player.playerInput.LCDown += LClickDown;
            _gameManager.player.playerInput.LCUp += LClickUp;
        }));
    }

    private void OnDisable()
    {
        if (_gameManager != null)
        {
            _gameManager.player.playerInput.LCDown -= LClickDown;
            _gameManager.player.playerInput.LCUp -= LClickUp;
        }
    }
    private void OnDestroy()
    {
        if(_gameManager)
            _gameManager.windows.Remove(gameObject);
    }
    private void LClickUp()
    {
        if (DetectSelf() && _clickLogo)
        {
            Instantiate(spawnWindow, _gameManager.canvas.transform);
            gameObject.SetActive(false);
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

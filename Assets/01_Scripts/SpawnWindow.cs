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
    private void OnEnable()
    {
        if(!_gameManager)
            _gameManager = GameManager.Instance;

        _gameManager.player.playerInput.LCDown += LClickDown;
        _gameManager.player.playerInput.LCUp += LClickUp;
    }

    private void OnDisable()
    {
        _gameManager.player.playerInput.LCDown -= LClickDown;
        _gameManager.player.playerInput.LCUp -= LClickUp;
    }
    private void LClickUp()
    {
        if (DetectSelf() && _clickLogo)
            Instantiate(spawnWindow, _gameManager.canvas.transform);
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

        // ī�޶��� �߾� ��ũ�� ��ǥ���� ���� �������� ���� ����
        Ray ray = _cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        // ����ĳ��Ʈ ����
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, selfLayer))
        {
            // �ڱ� �ڽ��� ����Ǿ����� Ȯ��
            return hitInfo.collider.gameObject == gameObject;
        }

        return false;  // �ƹ��͵� ������� ����
    }
}

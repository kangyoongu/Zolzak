using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChangeFile : MonoBehaviour
{
    public LayerMask selfLayer;  // �ڽ��� ���Ե� ���̾ ������ ���̾� ����ũ
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

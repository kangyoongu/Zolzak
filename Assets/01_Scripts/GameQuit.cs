using UnityEngine;

public class GameQuit : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("���� ����");
        Application.Quit();

        // �����Ϳ��� ���� ���� ��� ���� �޽����� �߰������� ���
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

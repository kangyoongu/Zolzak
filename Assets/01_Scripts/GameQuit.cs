using UnityEngine;

public class GameQuit : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();

        // 에디터에서 실행 중일 경우 종료 메시지를 추가적으로 출력
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

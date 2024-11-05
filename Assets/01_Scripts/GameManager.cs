using UnityEngine;
using UnityEngine.Events;

public class GameManager : SingleTon<GameManager>
{
    public UnityEvent LockCursor;
    public UnityEvent UnlockCursor;
    public Player player;
    public Canvas canvas;
    public RenderTexture fullscreenTexture;
    [SerializeField] Camera _renderCam;
    void Start()
    {
        Core.ResetToDefaultCursor();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        FitWindowScene();
    }

    private void FitWindowScene()
    {
        // RenderTexture 생성
        Vector2 resolution = ScaleResolution(Screen.width, Screen.height);
        fullscreenTexture = new RenderTexture((int)resolution.x, (int)resolution.y, 24);
        fullscreenTexture.Create();

        // 카메라의 타겟 텍스처를 RenderTexture로 설정
        _renderCam.targetTexture = fullscreenTexture;
    }
    public Vector2 ScaleResolution(float x, float y)
    {
        float maxDimension = Mathf.Max(x, y);
        Vector2 resol = new Vector2(x, y);
        return maxDimension <= 1000f ? resol : resol * (1000f / maxDimension);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(Cursor.visible == false)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                UnlockCursor?.Invoke();
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                LockCursor?.Invoke();
            }
        }
    }
}

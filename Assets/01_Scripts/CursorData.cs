using UnityEngine;

public class CursorData : MonoBehaviour
{
    public Texture2D[] cursors;
    public void LockC()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}

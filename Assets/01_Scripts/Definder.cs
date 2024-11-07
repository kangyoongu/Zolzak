using UnityEngine;

public static class Definder
{
    private static Camera _mainCam;
    public static Camera MainCam { 
        get{
            if(!_mainCam)
                _mainCam = Camera.main;

            return _mainCam;
        }
        private set {
            _mainCam = value;
        }
    }
    private static CursorData cursor;
    public static CursorData Cursor
    {
        get
        {
            if (cursor == null)
                cursor = GameObject.FindAnyObjectByType<CursorData>();

            return cursor;
        }
        set => cursor = value;
    }
}

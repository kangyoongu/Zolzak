using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class Core
{
    [DllImport("user32.dll")]
    private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

    [DllImport("user32.dll")]
    private static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);

    private const int IDC_SIZENWSE = 32642; // NW-SE �밢�� Ŀ��
    private const int IDC_SIZENESW = 32643; // NE-SW �밢�� Ŀ��
    private const int IDC_SIZEWE = 32644;   // �¿� Ŀ��
    private const int IDC_SIZENS = 32645;   // ���� Ŀ��

    [StructLayout(LayoutKind.Sequential)]
    private struct ICONINFO
    {
        public bool fIcon;
        public int xHotspot;
        public int yHotspot;
        public IntPtr hbmMask;
        public IntPtr hbmColor;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct BITMAP
    {
        public int bmType;
        public int bmWidth;
        public int bmHeight;
        public int bmWidthBytes;
        public ushort bmPlanes;
        public ushort bmBitsPixel;
        public IntPtr bmBits;
    }

    [DllImport("gdi32.dll")]
    private static extern bool GetObject(IntPtr hObject, int nCount, ref BITMAP lpObject);

    private static Texture2D LoadCursorTexture(int cursorId)
    {
        IntPtr cursorHandle = LoadCursor(IntPtr.Zero, cursorId);
        if (cursorHandle == IntPtr.Zero)
        {
            Debug.LogError("Ŀ���� �ҷ��� �� �����ϴ�.");
            return null;
        }

        ICONINFO iconInfo;
        if (!GetIconInfo(cursorHandle, out iconInfo))
        {
            Debug.LogError("ICONINFO�� ������ �� �����ϴ�.");
            return null;
        }

        Texture2D cursorTexture = GetTextureFromIconInfo(iconInfo);
        DeleteObject(iconInfo.hbmMask);
        DeleteObject(iconInfo.hbmColor);

        return cursorTexture;
    }
    private static Texture2D GetTextureFromIconInfo(ICONINFO iconInfo)
    {
        Texture2D texture = new Texture2D(16, 16, TextureFormat.ARGB32, false);

        return texture;
    }

    // Ŀ�� ���� �Լ�
    public static void SetCursorNWSE() => SetCustomCursor(IDC_SIZENWSE);
    public static void SetCursorNESW() => SetCustomCursor(IDC_SIZENESW);
    public static void SetCursorWE() => SetCustomCursor(IDC_SIZEWE);
    public static void SetCursorNS() => SetCustomCursor(IDC_SIZENS);
    public static void ResetToDefaultCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    private static void SetCustomCursor(int cursorId)
    {
        Texture2D cursorTexture = LoadCursorTexture(cursorId);
        if (cursorTexture != null)
        {
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
    }
    public static int[] ShortToSignArray(short value)
    {
        int[] sign = new int[16];
        for (int i = 0; i < 16; i++)
        {
            sign[i] = (value & (1 << i)) != 0 ? 1 : -1;
        }
        return sign;
    }

    public static IEnumerator DelayFrame(Action action, int frame = 1)
    {
        for (int i = 0; i < frame; i++)
            yield return null;
        action?.Invoke();
    }
}

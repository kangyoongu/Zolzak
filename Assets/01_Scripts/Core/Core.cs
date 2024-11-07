using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class Core
{
    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);

    public const int NORMAL = 0; // NW-SE 대각선 커서
    public const int CLOCK12 = 1; // NE-SW 대각선 커서
    public const int CLOCK2 = 2;   // 좌우 커서
    public const int CLOCK3 = 3;   // 상하 커서
    public const int CLOCK5 = 4;   // 상하 커서

    private static Texture2D LoadCursorTexture(int cursorId)
    {
        return Definder.Cursor.cursors[cursorId];
    }
    public static void SetCustomCursor(int cursorId)
    {
        Texture2D cursorTexture = LoadCursorTexture(cursorId);
        if (cursorTexture != null)
        {
            Cursor.SetCursor(cursorTexture, Vector2.one * 10, CursorMode.Auto);
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
    public static float Remap(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return outputMin + (value - inputMin) * (outputMax - outputMin) / (inputMax - inputMin);
    }
}

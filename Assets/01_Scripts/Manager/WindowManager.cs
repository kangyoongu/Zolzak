using System;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : SingleTon<WindowManager>
{
    public RectTransform rect;
    public List<Transform> windows; 

    public void AddWindow(Transform window)
    {
        window.parent = rect;
        windows.Add(window);
    }
    public void RemoveWindow(Transform window)
    {
        windows.Remove(window);
    }
}

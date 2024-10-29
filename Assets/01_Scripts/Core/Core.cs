using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Core
{
    public static IEnumerator DelayFrame(Action action, int frame = 1)
    {
        for (int i = 0; i < frame; i++)
            yield return null;
        action?.Invoke();
    }
}

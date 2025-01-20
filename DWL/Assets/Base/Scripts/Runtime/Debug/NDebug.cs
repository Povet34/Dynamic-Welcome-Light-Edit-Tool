using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NDebug : MonoBehaviour
{
    private static bool isShow = true;

    public static void Log(object message)
    {
        if (isShow)
        {
            Debug.Log(message);
        }
    }

    public static void LogError(object message)
    {
        if (isShow)
        {
            Debug.LogError(message);
        }
    }

    public static void LogWarning(object message)
    {
        if (isShow)
        {
            Debug.LogWarning(message);
        }
    }
}

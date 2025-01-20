using UnityEngine;

public class ScreenSizer : MonoBehaviour
{
    void Awake()
    {
        Screen.SetResolution(1760, 990, false);
    }
}
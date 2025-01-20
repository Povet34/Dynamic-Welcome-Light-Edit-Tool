using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : AbstractSingleton<SettingManager>
{
    // 창 모드로 전환시, 이전 크기가 있다면 해당 크기로
    // 전체화면 모드로 전환시, 정해진 화면 크기로( 1920 x 1080 )
    // Unity Player 설정 => Resolution and Presentation => Allow Fullscreen Switch
    // 위 옵션을 사용하여(PC 환경), alt + enter로 전체화면 전환시, 화면 크기가 유지된다.( 전체화면이지만, 작은 크기 유지 )

    private const int SCREEN_WIDTH = 1920;
    private const int SCREEN_HEIGHT = 1080;
    private int previous_screen_width = 0;
    private int previous_screen_height = 0;

    public bool IsFullScreen()
    {
        return Screen.fullScreen;
    }

    public void SetFullScreen()
    {
        previous_screen_width = Screen.width;
        previous_screen_height = Screen.height;
        Screen.SetResolution(SCREEN_WIDTH, SCREEN_HEIGHT, true);
    }

    public void SetWindowScreen()
    {
        if (previous_screen_width != 0 && previous_screen_height != 0)
            Screen.SetResolution(previous_screen_width, previous_screen_height, false);
        else
            Screen.SetResolution(Screen.width, Screen.height, false);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Framework에서 기본적으로 사용하는 함수 API
/// </summary>
public interface IBase
{
    /// <summary>
    /// 초기화 시 사용되는 함수 
    /// </summary>
    void OnInit();
    /// <summary>
    /// 매 프레임 마다 호출되는 함수
    /// </summary>
    void OnUpdateFrame();
    /// <summary>
    /// 초 마다 호출되는 함수
    /// </summary>
    void OnUpdateSec();
    /// <summary>
    /// 활성화 될 때 호출되는 함수
    /// </summary>
    void OnActive();
    /// <summary>
    /// 비활성화 될 때 호출되는 함수
    /// </summary>
    void OnInactive();
    /// <summary>
    /// 메모리에서 삭제될 때 호출되는 함수
    /// </summary>
    void OnClear();
    /// <summary>
    /// Keyboard Esc or Android 뒤로가기 버튼 선택 시 호출되는 함수
    /// </summary>
    bool IsEscape();
}
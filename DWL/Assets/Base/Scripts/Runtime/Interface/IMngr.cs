using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 데이터를 관리하는 클래스는 Manager라는 네이밍과 함께 IManager를 상속 받아서 특정 함수를 구현한다.
/// </summary>
public interface IMngr
{
    /// <summary>
    /// App.cs -> 최초 시작 시 호출 되는 함수
    /// </summary>
    public void Init();

    /// <summary>
    /// App.cs -> Update() 매 프레임 마다 호출 되는 함수
    /// </summary>
    public void UpdateFrame();

    /// <summary>
    /// App.cs -> Loop() 1초마다 호출되는 함수 
    /// </summary>
    public void UpdateSec();

    /// <summary>
    /// Manager 클래스의 정보를 없앨때 사용하는 함수
    /// </summary>
    public void Clear();
}
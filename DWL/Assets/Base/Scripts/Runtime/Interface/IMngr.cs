using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����͸� �����ϴ� Ŭ������ Manager��� ���ְ̹� �Բ� IManager�� ��� �޾Ƽ� Ư�� �Լ��� �����Ѵ�.
/// </summary>
public interface IMngr
{
    /// <summary>
    /// App.cs -> ���� ���� �� ȣ�� �Ǵ� �Լ�
    /// </summary>
    public void Init();

    /// <summary>
    /// App.cs -> Update() �� ������ ���� ȣ�� �Ǵ� �Լ�
    /// </summary>
    public void UpdateFrame();

    /// <summary>
    /// App.cs -> Loop() 1�ʸ��� ȣ��Ǵ� �Լ� 
    /// </summary>
    public void UpdateSec();

    /// <summary>
    /// Manager Ŭ������ ������ ���ٶ� ����ϴ� �Լ�
    /// </summary>
    public void Clear();
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Framework���� �⺻������ ����ϴ� �Լ� API
/// </summary>
public interface IBase
{
    /// <summary>
    /// �ʱ�ȭ �� ���Ǵ� �Լ� 
    /// </summary>
    void OnInit();
    /// <summary>
    /// �� ������ ���� ȣ��Ǵ� �Լ�
    /// </summary>
    void OnUpdateFrame();
    /// <summary>
    /// �� ���� ȣ��Ǵ� �Լ�
    /// </summary>
    void OnUpdateSec();
    /// <summary>
    /// Ȱ��ȭ �� �� ȣ��Ǵ� �Լ�
    /// </summary>
    void OnActive();
    /// <summary>
    /// ��Ȱ��ȭ �� �� ȣ��Ǵ� �Լ�
    /// </summary>
    void OnInactive();
    /// <summary>
    /// �޸𸮿��� ������ �� ȣ��Ǵ� �Լ�
    /// </summary>
    void OnClear();
    /// <summary>
    /// Keyboard Esc or Android �ڷΰ��� ��ư ���� �� ȣ��Ǵ� �Լ�
    /// </summary>
    bool IsEscape();
}
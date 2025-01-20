using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������Ӹ��� ManagerUpdate�� ȣ��Ǹ鼭 �ػ��� ������ �ִ��� üũ�ϴ� Ŭ����
/// ��� ��ũ���� ������ �ִ� �ܸ��⿡�� �ػ��� ��ȭ�� ����
/// </summary>
public class ScreenMngr : MonoBehaviour, IMngr
{
    /// <summary>
    /// ScreenManager�� ���� ������� �����ϴ� ���� 
    /// </summary>
    Subject _subject;
    public Subject subject
    {
        get
        {
            if (_subject == null)
            {
                _subject = new Subject();
            }

            return _subject;
        }
    }
    /// <summary>
    /// ������ ������ �ִ� ȭ���� ���� ũ��
    /// </summary>
    public int ScreenWidth { get; private set; } = 0;

    /// <summary>
    /// ������ ������ �ִ� ȭ���� ���� ũ��
    /// </summary>
    public int ScreenHeight { get; private set; } = 0;

    /// <summary>
    /// ScreenManager�� ó�� ȣ��Ǹ� ȭ���� ũ�⸦ üũ
    /// </summary>
    public ScreenMngr()
    {
        UpdateFrame();
    }

    /// <summary>
    /// ȭ���� ũ�⿡ ������ ���� ��� �����ڵ鿡�� ȭ���� ������ ������ �˷���
    /// </summary>
    public void UpdateFrame()
    {
        if (ScreenWidth != Screen.width || ScreenHeight != Screen.height)
        {
            ScreenWidth = Screen.width;
            ScreenHeight = Screen.height;

            subject.OnNotify();
        }
    }

    public void Init() { }
    public void UpdateSec() { }

    public void Clear()
    {
        if (_subject != null)
        {
            _subject.OnClear();
        }
    }
}
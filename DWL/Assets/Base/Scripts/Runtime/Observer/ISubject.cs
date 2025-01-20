using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ��� : �����ڰ� �����ϴ� ��� 
/// </summary>
public interface ISubject
{
    /// <summary>
    /// �ڽ��� ������ �����ڸ� ���
    /// </summary>
    /// <param name="observer">������</param>
    void AddObserver(IObserver observer);
    /// <summary>
    /// �ڽ��� �����ϴ� �����ڸ� ����
    /// </summary>
    /// <param name="observer"></param>
    void RemoveObserver(IObserver observer);

    /// <summary>
    /// ���� ����� ��ȭ�� �߻����� �� �����ڵ鿡�� �˷���
    /// </summary>
    void OnNotify();

    /// <summary>
    /// ���� ����� ������ �� ��ϵ� �����ڸ� ����
    /// </summary>
    void OnClear();
}
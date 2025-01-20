using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������� �⺻ ������ �����ϱ� ���� �������̽�
/// </summary>
public interface IObserver
{
    /// <summary>
    /// �������� ���̵�
    /// </summary>
    int ID { get; set; }

    /// <summary>
    /// ���� ������κ��� ������ ���� ��, �ٸ� ������ ���� �켱������ ������ ������ ���� �� �ֵ��� �ϱ� ���� ��
    /// </summary>
    int Priority { get; set; }

    /// <summary>
    /// ���� ������κ��� ������ ���� ��� ȣ��Ǵ� �Լ� 
    /// </summary>
    void OnResponse(object obj);
}
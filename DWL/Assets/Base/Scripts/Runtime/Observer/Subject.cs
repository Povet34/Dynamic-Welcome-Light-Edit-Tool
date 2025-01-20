using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ����� �⺻ ����
/// </summary>
public class Subject : ISubject
{
    /// <summary>
    /// �����ڸ� �����ϴ� ����Ʈ
    /// </summary>
    List<IObserver> observers;

    /// <summary>
    /// �����ڰ� �߰��� �� �ο��Ǵ� ���̵�
    /// </summary>
    int observerId = 0;

    /// <summary>
    /// �����ڸ� �߰�
    /// </summary>
    /// <param name="addObserver">�߰��� ������</param>
    public void AddObserver(IObserver addObserver)
    {
        if (observers == null)
        {
            observers = new List<IObserver>();
            observerId = 0;
        }

        addObserver.ID = observerId++;
        observers.Add(addObserver);
        observers.Sort(ComparePriority);
    }

    /// <summary>
    /// ������ priority�� ���� �������� ������ ����
    /// </summary>
    int ComparePriority(IObserver a, IObserver b)
    {
        return a.Priority.CompareTo(b.Priority);
    }

    /// <summary>
    /// �����ڸ� ����
    /// </summary>
    /// <param name="removeObserver">������ ������</param>
    public void RemoveObserver(IObserver removeObserver)
    {
        IObserver observer;
        for (int i = observers.Count - 1; i >= 0; i--)
        {
            observer = observers[i];
            if (observer.ID.Equals(removeObserver.ID))
            {
                observers.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// ���� ��󿡰� ��ȭ�� ���� ��� ȣ��Ǵ� �Լ�
    /// </summary>
    public void OnNotify()
    {
        if (observers != null)
        {
            IObserver obserber;
            for (int i = 0, icount = observers.Count; i < icount; i++)
            {
                obserber = observers[i];
                if (obserber != null)
                {
                    obserber.OnResponse(this);
                }
            }
        }
    }

    /// <summary>
    /// ���� ����� ������ �� ȣ��Ǵ� �Լ�
    /// </summary>
    public void OnClear()
    {
        if (observers != null)
        {
            observers.Clear();
        }
        observers = null;
        observerId = 0;
    }
}
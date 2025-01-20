using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Dictionary�� Unity���� Serialize�� �������� �ʱ� ������ Serialize�� ������ class�� ����
/// </summary>
/// <typeparam name="K"> dictinary���� Key�� ����� ���� Ÿ��</typeparam>
/// <typeparam name="V"> dictinary���� Value�� ����� ���� Ÿ��</typeparam>
[Serializable]
public class SerializeDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
{
    [SerializeField]
    List<K> keys = new List<K>();

    [SerializeField]
    List<V> values = new List<V>();

    /// <summary>
    /// Serialize�� �Ǳ����� �Ҹ��� �Լ�, �����͸� �����ϴ� �뵵�� ���
    /// </summary>
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (KeyValuePair<K, V> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    /// <summary>
    /// Serialize�� �� �� �Ҹ��� �Լ�, �����͸� �ҷ����� �뵵�� ���
    /// </summary>
    public void OnAfterDeserialize()
    {
        this.Clear();

        for (int i = 0, icount = keys.Count; i < icount; ++i)
        {
            this.Add(keys[i], values[i]);
        }
    }
}
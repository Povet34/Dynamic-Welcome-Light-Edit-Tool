using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ӿ��� ����� �����͸� �Ϻ�ȣȭ�ϴ� Ŭ���� 
/// </summary>
/// <typeparam name="T"></typeparam>
public class CryptoValue<T>
{
    /// <summary>
    /// ��ȣȭ�� ���ڿ��� �����ϱ� ���� ���� 
    /// </summary>
    string encryptData = string.Empty;

    /// <summary>
    /// ���� ������ 
    /// </summary>
    T data;

    /// <summary>
    /// ��ȣȭ ������ ���� �ִ� �Լ�
    /// </summary>
    /// <param name="value"> �� </param>
    public void Set(T value)
    {
        encryptData = App.Instance.Crypto.EncryptAESbyBase64Key(value.ToString());
        data = value;
    }

    /// <summary>
    /// ��ȣȭ�� �ʿ����� ���� ������ ����ϴ� Get �Լ���, UI�� �ܼ� ǥ�� �뵵�� ����� �� ���� 
    /// </summary>
    /// <returns></returns>
    public T GetUnSafeData()
    {
        return (T)Convert.ChangeType(data, typeof(T));
    }

    /// <summary>
    /// �߿��ϰ� ó���Ǵ� �������� ����ϴ� Get �Լ���, ���� ���� ���� ���� ������ ����� �� ���� 
    /// </summary>
    /// <returns></returns>
    public T Get()
    {
        return (T)Convert.ChangeType(App.Instance.Crypto.DecryptAESByBase64Key(encryptData), typeof(T));
    }
}
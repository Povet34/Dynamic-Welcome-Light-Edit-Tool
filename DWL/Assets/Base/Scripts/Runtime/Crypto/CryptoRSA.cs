using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Windows;

/// <summary>
/// RSA �Ϻ�ȣȭ�� ����ϴ� Ŭ���� 
/// </summary>
public class CryptoRSA : MonoBehaviour
{
    /// <summary>
    /// RSA ��ȣȭ�� ����� Ŭ����
    /// </summary>
    RSACryptoServiceProvider encrypter = null;
    /// <summary>
    /// RSA ��ȣȭ�� ����� Ŭ���� 
    /// </summary>
    RSACryptoServiceProvider decrypter = null;

    /// <summary>
    /// RSA �Ϻ�ȣȭ Ŭ���� ���� 
    /// </summary>
    /// <param name="base64PublicKey"> base64 ���� Ű</param>
    /// <param name="base64PrivateKey"> base64 ���� Ű</param>
    public void Create(string base64PublicKey, string base64PrivateKey)
    {
        encrypter = new RSACryptoServiceProvider();
        encrypter.FromXmlString(Crypto.DecodingBase64(base64PublicKey));

        decrypter = new RSACryptoServiceProvider();
        decrypter.FromXmlString(Crypto.DecodingBase64(base64PrivateKey));
    }

    /// <summary>
    /// RSA�� ��ȣȭ�ϴ� �Լ� 
    /// </summary>
    /// <param name="plainText"> RSA�� ��ȣȭ�� ���ڿ�</param>
    /// <returns></returns>
    public string Encrypt(string plainText)
    {
        byte[] byteData = Encoding.UTF8.GetBytes(plainText);
        byte[] byteEncrypt = encrypter.Encrypt(byteData, false);

        return Convert.ToBase64String(byteEncrypt);
    }

    /// <summary>
    /// RSA�� ��ȣȭ�� ���ڿ��� ��ȣȭ�ϴ� �Լ�
    /// </summary>
    /// <param name="encryptData"> RSA�� ��ȣȭ�� ���ڿ� </param>
    /// <returns></returns>
    public string Decrypt(string encryptData)
    {
        byte[] byteEncrypt = Convert.FromBase64String(encryptData);
        byte[] byteData = decrypter.Decrypt(byteEncrypt, false);

        return Encoding.UTF8.GetString(byteData);
    }
}
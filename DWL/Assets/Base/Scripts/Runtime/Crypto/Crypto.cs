using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

/// <summary>
/// ���ӿ��� �Ϻ�ȣȭ ����� �����ϴ� Ŭ���� 
/// </summary>
public class Crypto : MonoBehaviour
{
    /// <summary>
    /// base64�� ���ڿ��� ����� �Լ�
    /// </summary>
    /// <param name="plainText"> base64�� ������ ���ڿ�</param>
    /// <returns></returns>
    public static string EncodingBase64(string plainText)
    {
        Byte[] strByte = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(strByte);
    }

    /// <summary>
    /// base64 ���ڿ��� ���� ���ڿ��� ����� �Լ� 
    /// </summary>
    /// <param name="base64PlainText"> base64�� ��ȯ�� ���ڿ�</param>
    /// <returns></returns>
    public static string DecodingBase64(string base64PlainText)
    {
        Byte[] strByte = Convert.FromBase64String(base64PlainText);
        return Encoding.UTF8.GetString(strByte);
    }

    /// <summary>
    /// �ؽ� �Լ��� ����ϴ� SHA256�� �����ϱ� ���� static ������ ���� 
    /// </summary>
    static SHA256 _sha256 = null;

    /// <summary>
    /// ���ڿ��� sha256 �ؽ� ������ ����� �Լ�
    /// </summary>
    /// <param name="plainText"> sha256 �ؽ� ������ ������ ���ڿ� </param>
    /// <returns></returns>
    public static string SHA256Base64(string plainText)
    {
        if (_sha256 == null)
        {
            _sha256 = new SHA256Managed();
        }

        Byte[] hash = _sha256.ComputeHash(Encoding.UTF8.GetBytes(plainText));
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// ��ȣȭ�� ����� AESŬ������ �����ϱ� ���� Dictionary�� ���� 
    /// </summary>
    static Dictionary<string, CryptoAES> _aesManages = new Dictionary<string, CryptoAES>();

    /// <summary>
    /// AES ��ȣȭ�� ����� Ŭ���� ����
    /// </summary>
    /// <param name="base64Key"> aes�� ����� base64 key </param>
    /// <param name="base64IV"> aes�� ����� base64 iv </param>
    static void CreateAESManage(string base64Key, string base64IV)
    {
        CryptoAES aesManage = new CryptoAES();
        aesManage.Create(base64Key, base64IV);
        _aesManages.Add(base64Key, aesManage);
    }

    /// <summary>
    /// ���ڿ��� AES�� ��ȣȭ�ϴ� �Լ� 
    /// </summary>
    /// <param name="plainText"> ��ȣȭ�� ���ڿ� </param>
    /// <param name="base64Key"> ��ȣȭ�� ����� base64 key </param>
    /// <param name="base64IV"> ��ȣȭ�� ����� base64 iv</param>
    /// <returns></returns>
    public static string EncryptAESbyBase64Key(string plainText, string base64Key, string base64IV)
    {
        if (_aesManages.ContainsKey(base64Key) == false)
        {
            CreateAESManage(base64Key, base64IV);
        }

        return _aesManages[base64Key].Encrypt(plainText);
    }

    /// <summary>
    /// AES�� ��ȣȭ�� ���ڿ��� ��ȣȭ�Ͽ� ���� ���ڿ��� ��ȯ�ϴ� �Լ� 
    /// </summary>
    /// <param name="encryptData"> AES�� ��ȣȭ�� ���ڿ�</param>
    /// <param name="base64Key"> ��ȣȭ �Ҷ� ����ߴ� base64 key </param>
    /// <param name="base64IV"> ��ȣȭ �� �� ����ߴ� base64 iv </param>
    /// <returns></returns>
    public static string DecryptAESByBase64Key(string encryptData, string base64Key, string base64IV)
    {
        if (_aesManages.ContainsKey(base64Key) == false)
        {
            CreateAESManage(base64Key, base64IV);
        }

        return _aesManages[base64Key].Decrypt(encryptData);
    }

    /// <summary>
    /// RSA ��ȣȭ�� ����� Ŭ������ �����ϱ� ���� Dictionary�� ���� 
    /// </summary>
    static Dictionary<string, CryptoRSA> _rsaManages = new Dictionary<string, CryptoRSA>();

    /// <summary>
    /// RSA �Ϻ�ȣȭ�� ����� Class�� �����ϴ� �Լ� 
    /// </summary>
    /// <param name="base64PublicKey"></param>
    /// <param name="base64PrivateKey"></param>
    static void CreateRSAManage(string base64PublicKey, string base64PrivateKey)
    {
        CryptoRSA rsaManage = new CryptoRSA();
        rsaManage.Create(base64PublicKey, base64PrivateKey);
        _rsaManages.Add(base64PublicKey, rsaManage);
    }

    /// <summary>
    /// ���ڿ��� RSA�� ��ȣȭ�ϴ� �Լ�
    /// </summary>
    /// <param name="plainText"> ��ȣȭ�� ���ڿ� </param>
    /// <param name="base64PublicKey"> ��ȣȭ�� ����� base64 ���� Ű </param>
    /// <param name="base64PrivateKey"> ��ȣȭ�� ����� basse64 ���� Ű</param>
    /// <returns></returns>
    public static string EncryptRSAbyBase64PublicKey(string plainText, string base64PublicKey, string base64PrivateKey)
    {
        if (_rsaManages.ContainsKey(base64PublicKey) == false)
        {
            CreateRSAManage(base64PublicKey, base64PrivateKey);
        }

        return _rsaManages[base64PublicKey].Encrypt(plainText);
    }

    /// <summary>
    /// RSA�� ��ȣȭ�� ���ڿ��� ��ȣȭ�ϴ� �Լ� 
    /// </summary>
    /// <param name="encryptData"> RSA�� ��ȣȭ�� ���ڿ� </param>
    /// <param name="base64PublicKey"> RSA�� ��ȣȭ�Ҷ� ����� ���� Ű </param>
    /// <param name="base64PrivateKey"> RSA�� ��ȣȣȰ �� ����� ���� Ű </param>
    /// <returns></returns>
    public static string DecryptRSAByBase64Key(string encryptData, string base64PublicKey, string base64PrivateKey)
    {
        if (_rsaManages.ContainsKey(base64PublicKey) == false)
        {
            CreateRSAManage(base64PublicKey, base64PrivateKey);
        }

        return _rsaManages[base64PublicKey].Decrypt(encryptData);
    }
}

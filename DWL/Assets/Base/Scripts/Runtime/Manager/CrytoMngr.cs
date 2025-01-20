using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using Neofect.Utility;

/// <summary>
/// Crypto Ŭ������ �����ؼ� ����ϴ� �Ϻ�ȣȭ ���� Ŭ���� 
/// </summary>
public class CryptoMngr : MonoBehaviour, IMngr
{
    /// <summary>
    /// �Ϻ�ȣȭ�� ����� Ű���� ��� �ִ� Scriptable Object
    /// </summary>
    public CryptoData cryptoData;

    /// <summary>
    /// ���ڿ��� base64�� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="plainText"> base64�� ��ȯ�� ���ڿ�</param>
    /// <returns></returns>
    public string EncodingBase64(string plainText)
    {
        return Crypto.EncodingBase64(plainText);
    }

    /// <summary>
    /// base64�� �� ���ڿ��� ���� ���ڿ��� �����ϴ� �Լ� 
    /// </summary>
    /// <param name="base64PlainText">base64�� ��ȯ�� ���ڿ�</param>
    /// <returns></returns>
    public string DecodingBase64(string base64PlainText)
    {
        return Crypto.DecodingBase64(base64PlainText);
    }

    /// <summary>
    /// ���ڿ��� sha256 �ؽ� ���ڿ��� ��ȯ�ϴ� �Լ� 
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public string SHA256Base64(string plainText)
    {
        return Crypto.SHA256Base64(plainText);
    }

    /// <summary>
    /// ���ڿ��� AES�� ��ȣȭ�ϴ� �Լ�
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public string EncryptAESbyBase64Key(string plainText)
    {
        return Crypto.EncryptAESbyBase64Key(plainText, cryptoData.aesBase64Key, cryptoData.aesBase64IV);
    }

    /// <summary>
    /// AES�� ��ȣȭ�� ���ڿ��� ���� ���ڿ��� ��ȣȭ�ϴ� �Լ� 
    /// </summary>
    /// <param name="encryptData"></param>
    /// <returns></returns>
    public string DecryptAESByBase64Key(string encryptData)
    {
        return Crypto.DecryptAESByBase64Key(encryptData, cryptoData.aesBase64Key, cryptoData.aesBase64IV);
    }

    /// <summary>
    /// ���ڿ��� RSA�� ��ȣȭ �ϴ� �Լ� 
    /// </summary>
    /// <param name="plainText">��ȣȭ�� ���ڿ�</param>
    /// <returns></returns>
    public string EncryptRSAbyBase64PublicKey(string plainText)
    {
        return Crypto.EncryptRSAbyBase64PublicKey(plainText, cryptoData.rsaBase64PublicKey, cryptoData.rsaBase64PrivateKey);
    }

    /// <summary>
    /// RSA�� ��ȣȭ�� ���ڿ��� ��ȣȭ�ϴ� �Լ�
    /// </summary>
    /// <param name="encryptData"> RSA�� ��ȣȭ�� ���ڿ�</param>
    /// <returns></returns>
    public string DecryptRSAByBase64Key(string encryptData)
    {
        return Crypto.DecryptRSAByBase64Key(encryptData, cryptoData.rsaBase64PublicKey, cryptoData.rsaBase64PrivateKey);
    }

    /// <summary>
    /// playerPref�� data�� ��ȣȭ�Ͽ� �����ϴ� �Լ�
    /// </summary>
    /// <param name="key"> ���� �� ����� Ű��</param>
    /// <param name="data">���� �� ������</param>
    public void SavePlayerPrefsAESEncrpy(string key, string data)
    {
        FileUtility.SavePlayerPrefs(key, data, cryptoData.aesBase64Key, cryptoData.aesBase64IV);
    }

    /// <summary>
    /// playerPrefs�� ����� ��ȣȭ�� �����͸� ��ȣȭ�ϴ� �Լ�
    /// </summary>
    /// <param name="key"> ���� �� ����� Ű��</param>
    /// <param name="defualtData"> �����Ͱ� ���� ��� ����� �⺻ ������ ��</param>
    /// <returns></returns>
    public string SavePlayerPrefsAESDecrpy(string key, string defualtData)
    {
        return FileUtility.GetPlayerPrefs(key, defualtData, cryptoData.aesBase64Key, cryptoData.aesBase64IV);
    }

    /// <summary>
    /// ���Ͽ� �����͸� AES�� ��ȣȭ�Ͽ� �����ϴ� �Լ� 
    /// </summary>
    /// <param name="folderPath">������ ������ ���</param>
    /// <param name="fileName">������ ������ �̸�</param>
    /// <param name="extention">������ ������ Ȯ����</param>
    /// <param name="data">������ ������</param>
    public void WriteFileAESEncrpy(string folderPath, string fileName, string extention, string data)
    {
        FileUtility.WriteFile(folderPath, fileName, extention, data, cryptoData.aesBase64Key, cryptoData.aesBase64IV);
    }

    /// <summary>
    /// AES ��ȣȭ�� ����� ������ ��ȣȭ�ϴ� �Լ�
    /// </summary>
    /// <param name="folderPath">����� ������ ���</param>
    /// <param name="fileName">����� ������ �̸�</param>
    /// <param name="extention">����� ������ Ȯ����</param>
    /// <returns></returns>
    public string ReadFileAESDrcpy(string folderPath, string fileName, string extention)
    {
        return FileUtility.ReadFile(folderPath, fileName, extention, cryptoData.aesBase64Key, cryptoData.aesBase64IV);
    }

    public void Init() { }
    public void UpdateFrame() { }
    public void UpdateSec() { }
    public void Clear() { }
}

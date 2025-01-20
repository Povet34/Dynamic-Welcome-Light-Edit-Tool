using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

/// <summary>
/// AES�� �Ϻ�ȣȭ �� ����ϴ� Ŭ���� 
/// </summary>
public class CryptoAES
{
    /// <summary>
    /// AES�� Ű�� ���̴� 128, 192, 256 ���� ����
    /// </summary>
    public static int[] aesKeySize = { 128, 192, 256 };
    /// <summary>
    /// AES�� �ʱ� ������ ���̴� 128�� ��� 
    /// </summary>
    public static int aesIVSize = 128;

    /// <summary>
    /// AES ��ȣȭ�� ����ϴ� Ŭ����
    /// </summary>
    ICryptoTransform encrypter;
    /// <summary>
    /// AES ��ȣȭ�� ����ϴ� Ŭ���� 
    /// </summary>
    ICryptoTransform decrypter;

    /// <summary>
    /// AES �Ϻ�ȣȭ�� ����� Ŭ������ �����ϴ� �Լ� 
    /// </summary>
    /// <param name="base64Key"> AES Ŭ�������� ����� base64 Ű ��</param>
    /// <param name="base64IV"> AES Ŭ�������� ����� base64 �ʱ� ���� ��</param>
    public void Create(string base64Key, string base64IV)
    {
        byte[] key = Convert.FromBase64String(base64Key);
        byte[] iv = Convert.FromBase64String(base64IV);

        RijndaelManaged rijndaelManaged = new RijndaelManaged();
        rijndaelManaged.KeySize = key.Length * 8;
        rijndaelManaged.BlockSize = aesIVSize;
        rijndaelManaged.Padding = PaddingMode.PKCS7;
        rijndaelManaged.Mode = CipherMode.CBC;

        rijndaelManaged.Key = key;
        rijndaelManaged.IV = iv;

        encrypter = rijndaelManaged.CreateEncryptor();
        decrypter = rijndaelManaged.CreateDecryptor();
    }

    /// <summary>
    /// AES�� ��ȣȭ�ϴ� �Լ� 
    /// </summary>
    /// <param name="plainText"> AES�� ��ȣȭ�� ���ڿ� </param>
    /// <returns></returns>
    public string Encrypt(string plainText)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encrypter, CryptoStreamMode.Write))
            {
                byte[] byteData = Encoding.UTF8.GetBytes(plainText);
                cryptoStream.Write(byteData, 0, byteData.Length);
            }

            byte[] byteCrypto = memoryStream.ToArray();
            return Convert.ToBase64String(byteCrypto);
        }
    }

    /// <summary>
    /// AES�� �� ���ڿ��� ��ȣȭ�ϴ� �Լ� 
    /// </summary>
    /// <param name="encryptData"> AES�� ��ȣȭ�� ���ڿ� </param>
    /// <returns></returns>
    public string Decrypt(string encryptData)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decrypter, CryptoStreamMode.Write))
            {
                byte[] byteEncrpt = Convert.FromBase64String(encryptData);
                cryptoStream.Write(byteEncrpt, 0, byteEncrpt.Length);
            }

            byte[] byteCrypto = memoryStream.ToArray();
            return Encoding.UTF8.GetString(byteCrypto);
        }
    }
}
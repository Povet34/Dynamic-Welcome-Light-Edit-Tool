using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 암호화에 사용하는 키 값을 관리하는 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "CryptoData", menuName = "Scriptable Object Asset/CryptoData")]
public class CryptoData : ScriptableObject
{
    [HideInInspector]
    public string aesBase64Key = string.Empty;
    [HideInInspector]
    public string aesBase64IV = string.Empty;

    [HideInInspector]
    public string folderPath = string.Empty;
    [HideInInspector]
    public string fileName = string.Empty;

    [HideInInspector]
    public string rsaBase64PublicKey = string.Empty;
    [HideInInspector]
    public string rsaBase64PrivateKey = string.Empty;
}
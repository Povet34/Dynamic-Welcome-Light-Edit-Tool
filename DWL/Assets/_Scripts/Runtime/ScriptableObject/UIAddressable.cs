using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum eUIType
{
    None = 0,
    Login = 1,          /// �α���
    TERMS_CONDITIONS,   /// ��� ����
    MEMBERSHIP_JOIN,    /// ȸ������
    SELF_CERTIFICATION, /// ��������
    FIND,               /// ID / PW ã��
    NavigationBar,      /// Ȩ �ϴ� �׺���̼� ��
    Home,               /// Ȩ
    Record,             /// ����ϱ�
    NOTICE,             /// ��������
    WaterIntake,        /// ���м���
    KidneyFunction,     /// �ű��
    BloodPressure,      /// ����
    Weight,             /// ü��
    Symptom,            /// ����

    PDA_WRITE,
    PAD_INQUIRY,        
}

/// <summary>
/// ���ӿ��� ����ϴ� UI �� �����ϴ� ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "UIAddressable", menuName = "Scriptable Object Asset/UIAddressable")]
public class UIAddressable : ScriptableObject
{
    /// <summary>
    /// ���ӿ��� ����ϴ� UI
    /// </summary>
    public List<UIAddressableData> uiList = new List<UIAddressableData>();

    public void SetUIAddressableDataByUIType(eUIType uiType, bool isIgnoreEscape, bool isFullScreen, bool isCaching, AssetReference reference)
    {
        bool isExist = false;
        UIAddressableData data;
        for (int index = 0; index < uiList.Count; index++) 
        {
            if ((data = uiList[index]) != null && data.uiType.Equals(uiType))
            {
                data.isIgnoreEscape = isIgnoreEscape;
                data.isFullScreen = isFullScreen;
                data.isCaching = isCaching;
                data.reference = reference;
                isExist = true;
            }
        }

        if (!isExist)
        {
            UIAddressableData newData = new UIAddressableData();
            newData.uiType = uiType;
            newData.isIgnoreEscape = isIgnoreEscape;
            newData.isFullScreen = isFullScreen;
            newData.isCaching = isCaching;
            newData.reference = reference;
            uiList.Add(newData);
        }

        //EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// eUIType�� �ش��ϴ� UI �����͸� ��ȯ
    /// </summary>
    public UIAddressableData GetUIAddressableDataByUIType(eUIType uiType)
    {
        UIAddressableData data;
        for (int index = 0, icount = uiList.Count; index < icount; ++index)
        {
            if ((data = uiList[index]) != null && data.uiType.Equals(uiType))
            {
                return data;
            }
        }

        return null;
    }
}

/// <summary>
/// Addressable���� ����� UI�� ������
/// </summary>
[Serializable]
public class UIAddressableData
{
    public eUIType uiType;
    public bool isFullScreen;
    public bool isCaching;
    public bool isIgnoreEscape;
    public int fixedSortingOrder;
    public AssetReference reference;
}
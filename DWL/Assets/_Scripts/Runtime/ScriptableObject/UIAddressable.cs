using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum eUIType
{
    None = 0,
    Login = 1,          /// 로그인
    TERMS_CONDITIONS,   /// 약관 동의
    MEMBERSHIP_JOIN,    /// 회원가입
    SELF_CERTIFICATION, /// 본인인증
    FIND,               /// ID / PW 찾기
    NavigationBar,      /// 홈 하단 네비게이션 바
    Home,               /// 홈
    Record,             /// 기록하기
    NOTICE,             /// 공지사항
    WaterIntake,        /// 수분섭취
    KidneyFunction,     /// 신기능
    BloodPressure,      /// 혈압
    Weight,             /// 체중
    Symptom,            /// 증상

    PDA_WRITE,
    PAD_INQUIRY,        
}

/// <summary>
/// 게임에서 사용하는 UI 를 관리하는 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "UIAddressable", menuName = "Scriptable Object Asset/UIAddressable")]
public class UIAddressable : ScriptableObject
{
    /// <summary>
    /// 게임에서 사용하는 UI
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
    /// eUIType에 해당하는 UI 데이터를 반환
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
/// Addressable에서 사용할 UI의 데이터
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
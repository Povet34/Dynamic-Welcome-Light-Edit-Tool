using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JsonIgnoreCondition
{
    Never = 0,
    Always = 1,
    WhenWritingDefault = 2,
    WhenWritingNull = 3
}

public sealed class JsonIgnoreAttribute : Attribute
{
    public JsonIgnoreCondition Condition { get; set; }
}

/// <summary>
/// API ��û ���� ���
/// </summary>
public class ApiRequest
{
    // HTTP Header�� �߰� Authorization : accountUid.sessionKey
    // public long accountUid; public string sessionKey; 

    public virtual string GetPath() { return string.Empty; }
}

/// <summary>
/// API ���� ���� ���
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// �����ڵ�
    /// </summary>
    public string code { get; set; }

    /// <summary>
    /// �����޽���
    /// </summary>
    public string message { get; set; }
}

#region Login �� User API : ----------------------------------------------------
public class CSSignIn : ApiRequest
{
    public override string GetPath() { return "/users/sign-in"; }
    public eRequestType requestType = eRequestType.Post;

    public string email;
    public string password;
}

[Serializable]
public class SCSignIn : ApiResponse
{
    public Info payload;

    public class Info
    {
        public string accessToken;
    }
}

public class CSUserFetch : ApiRequest
{
    public override string GetPath() { return "/users"; }
    public eRequestType requestType = eRequestType.Get;

    public CSUserFetch() { }
}

[Serializable]
public class SCUserFetch : ApiResponse
{
    public Info payload;

    public class Info
    {
        public string email { get; set; }
        public string name { get; set; }
        public string birthday { get; set; }
        public eGender gender { get; set; }
        public float height { get; set; }
        public float weight { get; set; }
        public string chronicDisease { get; set; }
        public string otherDisease { get; set; }
        public string hospital { get; set; }
        public string medicalStaff { get; set; }

        public string createdTime { get; set; }
        public string updatedTime { get; set; }
    }
}
#endregion

#region ��� ���� API : -----------------------------------------------------------
public class CSSummaryFetch : ApiRequest
{
    public override string GetPath() { return "/summary"; }
    public eRequestType requestType = eRequestType.Get;

    public string date;
}

[Serializable]
public class SCSummaryFetch : ApiResponse
{
    public Info payload;

    public class Info
    {
        public HydrationIntake hydrationIntake { get; set; }
        public KidneyFunction kidneyFunction { get; set; }
        public BloodPressure bloodPressure { get; set; }
        public int? weight { get; set; }
        public string symptoms { get; set; }
        
        //public int urineVolume { get; set; }
        // �� ���� �ʿ�
        // TKV �ʿ�

        public class HydrationIntake
        {
            public int goal { get; set; }
            public int totalVolume { get; set; }
        }

        public class KidneyFunction
        {
            public int egfr { get; set; }
            public float creatinine { get; set; }
        }

        public class BloodPressure
        {
            public int? systolic { get; set; }
            public int? diastolic { get; set; }
        }
    }
}
#endregion

#region ���м��뷮 ���� API : --------------------------------------------------------
public class CSWaterIntakeCreate : ApiRequest
{
    public override string GetPath() { return "/hydration-intake"; }
    public eRequestType requestType = eRequestType.Post;

    public string intakeTime;           // ���� ���� �ð� (ex : 2023-11-07 12:00:12
    public int volume;                  // 1ȸ ���� ���뷮 (ex : 200)
}

[Serializable]
public class SCWaterIntakeCreate : ApiResponse
{
    public Info payload;

    public class Info
    {
        public int goal;
        public int volume;
        public LastRecordWaterIntake[] list;
    }
}

public class CSWaterIntakeFetch : ApiRequest
{
    public override string GetPath() { return "/hydration-intake"; }
    public eRequestType requestType = eRequestType.Get;

    public string date;
}

[Serializable]
public class SCWaterIntakeFetch : ApiResponse
{
    public Info payload;

    public class Info
    {
        public int goal;
        public int totalVolume;
        public LastRecordWaterIntake[] list;
    }
}

public class CSWaterIntakeSetGoal : ApiRequest
{
    public override string GetPath() { return "/hydration-intake/goal"; }
    public eRequestType requestType = eRequestType.Put;

    public int volume;
}

[Serializable]
public class SCWaterIntakeSetGoal : ApiResponse
{
    public Info payload;

    public class Info
    {
        public int volume;
    }
}
#endregion
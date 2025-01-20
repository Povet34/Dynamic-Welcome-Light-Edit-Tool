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
/// API 요청 공용 헤더
/// </summary>
public class ApiRequest
{
    // HTTP Header에 추가 Authorization : accountUid.sessionKey
    // public long accountUid; public string sessionKey; 

    public virtual string GetPath() { return string.Empty; }
}

/// <summary>
/// API 응답 공용 헤더
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// 에러코드
    /// </summary>
    public string code { get; set; }

    /// <summary>
    /// 에러메시지
    /// </summary>
    public string message { get; set; }
}

#region Login 및 User API : ----------------------------------------------------
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

#region 요약 정보 API : -----------------------------------------------------------
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
        // 약 복용 필요
        // TKV 필요

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

#region 수분섭취량 관련 API : --------------------------------------------------------
public class CSWaterIntakeCreate : ApiRequest
{
    public override string GetPath() { return "/hydration-intake"; }
    public eRequestType requestType = eRequestType.Post;

    public string intakeTime;           // 수분 섭취 시간 (ex : 2023-11-07 12:00:12
    public int volume;                  // 1회 수분 섭취량 (ex : 200)
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
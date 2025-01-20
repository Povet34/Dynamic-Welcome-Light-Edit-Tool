using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIParameterStorage : AbstractSingleton<UIParameterStorage>
{
    public const string PARAM_KEY_SELECT_DATE_TIME = "SelectDateTime";

    private Dictionary<string, object> parameterDic;

    // 생성자를 private으로 설정하여 외부에서 인스턴스화를 방지
    private UIParameterStorage()
    {
        parameterDic = new Dictionary<string, object>();
    }

    // 파라미터 설정
    public void SetParameter(string key, object value)
    {
        parameterDic[key] = value;
    }

    // 파라미터 검색
    public object GetParameter(string key)
    {
        parameterDic.TryGetValue(key, out var value);
        return value;
    }

    // 파라미터 초기화
    public void ClearParameter(string key)
    {
        if (parameterDic.ContainsKey(key))
        {
            parameterDic.Remove(key);
        }
    }

    #region Select Date Time : ------------------------------------------------
    public void SetSelectDateTime(DateTime selectDateTime)
    {
        parameterDic[PARAM_KEY_SELECT_DATE_TIME] = selectDateTime;
    }

    public DateTime GetSelectDateTime()
    {
        parameterDic.TryGetValue(PARAM_KEY_SELECT_DATE_TIME, out var value);
        return Convert.ToDateTime(value);
    }
    #endregion
}
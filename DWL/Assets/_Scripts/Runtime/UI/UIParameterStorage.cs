using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIParameterStorage : AbstractSingleton<UIParameterStorage>
{
    public const string PARAM_KEY_SELECT_DATE_TIME = "SelectDateTime";

    private Dictionary<string, object> parameterDic;

    // �����ڸ� private���� �����Ͽ� �ܺο��� �ν��Ͻ�ȭ�� ����
    private UIParameterStorage()
    {
        parameterDic = new Dictionary<string, object>();
    }

    // �Ķ���� ����
    public void SetParameter(string key, object value)
    {
        parameterDic[key] = value;
    }

    // �Ķ���� �˻�
    public object GetParameter(string key)
    {
        parameterDic.TryGetValue(key, out var value);
        return value;
    }

    // �Ķ���� �ʱ�ȭ
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
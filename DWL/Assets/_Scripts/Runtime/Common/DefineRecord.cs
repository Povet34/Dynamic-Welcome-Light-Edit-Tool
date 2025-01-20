using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static partial class Define
{
    #region Blood Pressure : --------------------------------------------------
    public enum eBloodPressureState
    {
        None = -1,
        Hypotension,            // 저혈압
        Normal,                 // 정상
        PreHypertension,        // 고혈압 전 단계
        HypertensionStage1,     // 고혈압 1단계
        HypertensionStage2      // 고혈압 2단계
    }

    public static eBloodPressureState GetBloodPressureState(int systolic, int diastolic)
    {
        // 고혈압 2기: 수축기 혈압이 160mmHg 이상이거나 이완기 혈압이 100mmHg 이상
        if (systolic >= 160 || diastolic >= 100)
            return eBloodPressureState.HypertensionStage2;

        // 고혈압 1기: 수축기 혈압이 140mmHg 이상 160mmHg 미만이거나 이완기 혈압이 90mmHg 이상 100mmHg 미만
        if ((systolic >= 140 && systolic < 160) || (diastolic >= 90 && diastolic < 100))
            return eBloodPressureState.HypertensionStage1;

        // 고혈압 주의 (전단계 고혈압): 수축기 혈압이 120mmHg 이상 140mmHg 미만이거나 이완기 혈압이 80mmHg 이상 90mmHg 미만
        if ((systolic >= 120 && systolic < 140) || (diastolic >= 80 && diastolic < 90))
            return eBloodPressureState.PreHypertension;

        // 정상 혈압: 수축기 혈압이 90mmHg 이상 120mmHg 미만이고 이완기 혈압이 60mmHg 이상 80mmHg 미만
        if (systolic < 120 && diastolic < 80)
            return eBloodPressureState.Normal;

        // 저혈압: 수축기 혈압이 90mmHg 미만이거나 이완기 혈압이 60mmHg 미만
        if (systolic < 90 || diastolic < 60)
            return eBloodPressureState.Hypotension;

        return eBloodPressureState.Normal; // 기본값
    }

    public static string GetBloodPressureStateText(eBloodPressureState bloodPressureState)
    {
        return bloodPressureState switch
        {
            eBloodPressureState.Hypotension => App.Instance.Language.GetLanguageText("RECORD_BLOOD_PRESSURE_STATE_HYPOTENSION"),
            eBloodPressureState.Normal => App.Instance.Language.GetLanguageText("RECORD_BLOOD_PRESSURE_STATE_NORMAL"),
            eBloodPressureState.PreHypertension => App.Instance.Language.GetLanguageText("RECORD_BLOOD_PRESSURE_STATE_PREHYPERTENSION"),
            eBloodPressureState.HypertensionStage1 => App.Instance.Language.GetLanguageText("RECORD_BLOOD_PRESSURE_STATE_HYPERTENSIONSTAGE1"),
            eBloodPressureState.HypertensionStage2 => App.Instance.Language.GetLanguageText("RECORD_BLOOD_PRESSURE_STATE_HYPERTENSIONSTAGE2"),
            _ => string.Empty,
        };
    }

    public static Color GetBloodPressureStateColor(eBloodPressureState bloodPressureState)
    {
        Color color;
        switch (bloodPressureState)
        {
            case eBloodPressureState.Hypotension:
                {
                    ColorUtility.TryParseHtmlString("#06B4FF", out color);
                    return color;
                }
            case eBloodPressureState.Normal:
                {
                    ColorUtility.TryParseHtmlString("#17CB81", out color);
                    return color;
                }
            case eBloodPressureState.PreHypertension:
                {
                    ColorUtility.TryParseHtmlString("#FF8039", out color);
                    return color;
                }
            case eBloodPressureState.HypertensionStage1:
                {
                    ColorUtility.TryParseHtmlString("#EB5728", out color);
                    return color;
                }
            case eBloodPressureState.HypertensionStage2:
                {
                    ColorUtility.TryParseHtmlString("#DE0000", out color);
                    return color;
                }
            default: return Color.white;
        }
    }
    #endregion
}
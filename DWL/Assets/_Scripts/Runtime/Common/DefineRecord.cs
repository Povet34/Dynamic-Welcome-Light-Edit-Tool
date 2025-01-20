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
        Hypotension,            // ������
        Normal,                 // ����
        PreHypertension,        // ������ �� �ܰ�
        HypertensionStage1,     // ������ 1�ܰ�
        HypertensionStage2      // ������ 2�ܰ�
    }

    public static eBloodPressureState GetBloodPressureState(int systolic, int diastolic)
    {
        // ������ 2��: ����� ������ 160mmHg �̻��̰ų� �̿ϱ� ������ 100mmHg �̻�
        if (systolic >= 160 || diastolic >= 100)
            return eBloodPressureState.HypertensionStage2;

        // ������ 1��: ����� ������ 140mmHg �̻� 160mmHg �̸��̰ų� �̿ϱ� ������ 90mmHg �̻� 100mmHg �̸�
        if ((systolic >= 140 && systolic < 160) || (diastolic >= 90 && diastolic < 100))
            return eBloodPressureState.HypertensionStage1;

        // ������ ���� (���ܰ� ������): ����� ������ 120mmHg �̻� 140mmHg �̸��̰ų� �̿ϱ� ������ 80mmHg �̻� 90mmHg �̸�
        if ((systolic >= 120 && systolic < 140) || (diastolic >= 80 && diastolic < 90))
            return eBloodPressureState.PreHypertension;

        // ���� ����: ����� ������ 90mmHg �̻� 120mmHg �̸��̰� �̿ϱ� ������ 60mmHg �̻� 80mmHg �̸�
        if (systolic < 120 && diastolic < 80)
            return eBloodPressureState.Normal;

        // ������: ����� ������ 90mmHg �̸��̰ų� �̿ϱ� ������ 60mmHg �̸�
        if (systolic < 90 || diastolic < 60)
            return eBloodPressureState.Hypotension;

        return eBloodPressureState.Normal; // �⺻��
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
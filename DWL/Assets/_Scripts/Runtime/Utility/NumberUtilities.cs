using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

namespace Common.Utility
{
    public static class NumberUtilities
    {
        public static float ConvertToDecimalFormatAndRound(int number)
        {
            float result = number / 1000.0f;
            return Convert.ToSingle(Math.Round(result, 1, MidpointRounding.AwayFromZero));
        }

        public static string FormatToKoreanCurrency(string numberString)
        {
            try
            {
                long number = long.Parse(numberString);
                //return number.ToString("N0");
                return $"{number:N0}";
            }
            catch (FormatException)
            {
                // ��ȯ�� �����ϴ� ��� ���� �޽��� ��ȯ
                return "�߸��� ������ �����Դϴ�.";
            }
            catch (OverflowException)
            {
                // ���ڰ� �ʹ� ū ��� ���� �޽��� ��ȯ
                return "�Է��� ���ڰ� �ʹ� Ů�ϴ�.";
            }
        }
    }
}
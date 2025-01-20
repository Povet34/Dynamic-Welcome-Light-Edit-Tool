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
                // 변환에 실패하는 경우 에러 메시지 반환
                return "잘못된 형식의 숫자입니다.";
            }
            catch (OverflowException)
            {
                // 숫자가 너무 큰 경우 에러 메시지 반환
                return "입력한 숫자가 너무 큽니다.";
            }
        }
    }
}
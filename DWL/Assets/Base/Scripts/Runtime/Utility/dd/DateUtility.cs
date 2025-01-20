using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neofect.BodyChecker.Utility
{
    public class DateUtility
    {
        public static DateTime ConvertMilliSecondToDateTime(long? ms)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddMilliseconds(ms ?? 0).ToLocalTime();
            return dt;
        }

        public static string GetYYYYMMDD(long? ms)
        {
            DateTime dt = ConvertMilliSecondToDateTime(ms);
            return $"{dt.ToString("yyyy")}.{dt.ToString("MM")}.{dt.ToString("dd")}";
        }

        public static string GetYYYYMMDD_HHMM(long? ms, bool isParentheses = true)
        {
            DateTime dt = ConvertMilliSecondToDateTime(ms);
            if (isParentheses)
                return $"{dt.ToString("yyyy")}.{dt.ToString("MM")}.{dt.ToString("dd")} ({dt.ToString("HH")}:{dt.ToString("mm")})";
            else
                return $"{dt.ToString("yyyy")}.{dt.ToString("MM")}.{dt.ToString("dd")} {dt.ToString("HH")}:{dt.ToString("mm")}";
        }

        public static string GetYYYYMMDD_HHMM_2Line(long? ms)
        {
            DateTime dt = ConvertMilliSecondToDateTime(ms);
            return $"{dt.ToString("yyyy")}.{dt.ToString("MM")}.{dt.ToString("dd")}\n({dt.ToString("HH")}:{dt.ToString("mm")})";
        }

        public static string GetYYYYMMDD_HHMM_3Line(long? ms)
        {
            DateTime dt = ConvertMilliSecondToDateTime(ms);
            return $"{dt.ToString("yyyy")}\n{dt.ToString("MM")}.{dt.ToString("dd")}\n({dt.ToString("HH")}:{dt.ToString("mm")})";
        }

        public static string GetYYYYMMDD_HHMMSS(long? ms)
        {
            DateTime dt = ConvertMilliSecondToDateTime(ms);
            return $"{dt.ToString("yyyy")}.{dt.ToString("MM")}.{dt.ToString("dd")}({dt.ToString("HH")}:{dt.ToString("mm")}:{dt.ToString("ss")})";
        }

        public static string GetYYMMDD_Temp(string date)
        {
            // ex) date : yyyy_mm_dd_hh_mm_ss
            var str = date.Split('_');
            return $"{str[0]}.{str[1]}.{str[2]}";
        }
        public static string GetYYMMDD_HHMM(string date)
        {
            // ex) date : yyyy_mm_dd_hh_mm_ss
            var str = date.Split('_');
            return $"{str[0]}.{str[1]}.{str[2]} ({str[3]}:{str[4]})";
        }
        public static string GetYYMMDD_HHMM_2Lines(string date)
        {
            // ex) date : yyyy_mm_dd_hh_mm_ss
            var str = date.Split('_');
            return $"{str[0]}.{str[1]}.{str[2]}\n({str[3]}:{str[4]})";
        }
        public static string GetYYMMDD_HHMM_3Lines(string date)
        {
            // ex) date : yyyy_mm_dd_hh_mm_ss
            var str = date.Split('_');
            return $"{str[0]}\n{str[1]}.{str[2]}\n{str[3]}:{str[4]}";
        }
        public static string GetYYMMDD_HHMM_report(string date)
        {
            // ex) date : yyyy_mm_dd_hh_mm_ss
            var str = date.Split('_');
            return $"{str[0]}.{str[1]}.{str[2]} {str[3]}:{str[4]}";
        }
    }

}

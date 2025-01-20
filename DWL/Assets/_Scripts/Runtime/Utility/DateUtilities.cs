using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

namespace Common.Utility
{
    public static class DateUtilities
    {
        public static string GetCurrentWeekInfo(DateTime dateTime)
        {
            // 주의 시작을 일요일로 설정
            DayOfWeek firstDayOfWeek = DayOfWeek.Sunday;

            // 해당 월의 첫 날
            DateTime firstDayOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1);

            // 첫 주의 시작 일 계산
            int daysToFirstDayOfWeek = (7 + firstDayOfMonth.DayOfWeek - firstDayOfWeek) % 7;
            DateTime firstWeekStart = firstDayOfMonth.AddDays(-daysToFirstDayOfWeek);

            // 주차 계산
            int weekNumber = ((dateTime - firstWeekStart).Days / 7) + 1;
            return string.Format(App.Instance.Language.GetLanguageText("RECORD_DATE_WEEK_LABEL_VALUE"), dateTime.Month, weekNumber);
        }

        public static DateTime[] GetWeekDates(DateTime dateTime)
        {
            int currentDayOfWeek = Convert.ToInt32(dateTime.DayOfWeek);
            DateTime sunday = dateTime.AddDays(-currentDayOfWeek);
            DateTime[] weeks = new DateTime[7];

            for (int index = 0; index < 7; index++)
            {
                weeks[index] = sunday.AddDays(index);
            }

            return weeks;
        }

        public static DateTime GetFirstDayOfMonth(DateTime dateTime) 
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        public static DateTime GetLastDayOfMonth(DateTime dateTime)
        {
            int daysInMonth = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            return new DateTime(dateTime.Year, dateTime.Month, daysInMonth);
        }

        public static bool IsSameWeek(DateTime srcDateTime, DateTime dstDateTime)
        {
            // CultureInfo를 사용하여 현재 문화권의 달력 주차를 계산
            var ci = CultureInfo.CurrentCulture;
            Calendar cal = ci.Calendar;
            CalendarWeekRule cwr = ci.DateTimeFormat.CalendarWeekRule;
            DayOfWeek firstDayOfWeek = ci.DateTimeFormat.FirstDayOfWeek;

            // 각 날짜의 주차 계산
            int weekOfYear1 = cal.GetWeekOfYear(srcDateTime, cwr, firstDayOfWeek);
            int weekOfYear2 = cal.GetWeekOfYear(dstDateTime, cwr, firstDayOfWeek);

            // 같은 달과 같은 주차인지 확인
            return srcDateTime.Month == dstDateTime.Month && weekOfYear1 == weekOfYear2;
        }

        public static bool IsToday(DateTime date) 
        {
            return date.Date == DateTime.Today;
        }
    }
}
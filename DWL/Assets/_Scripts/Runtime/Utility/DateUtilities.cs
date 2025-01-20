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
            // ���� ������ �Ͽ��Ϸ� ����
            DayOfWeek firstDayOfWeek = DayOfWeek.Sunday;

            // �ش� ���� ù ��
            DateTime firstDayOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1);

            // ù ���� ���� �� ���
            int daysToFirstDayOfWeek = (7 + firstDayOfMonth.DayOfWeek - firstDayOfWeek) % 7;
            DateTime firstWeekStart = firstDayOfMonth.AddDays(-daysToFirstDayOfWeek);

            // ���� ���
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
            // CultureInfo�� ����Ͽ� ���� ��ȭ���� �޷� ������ ���
            var ci = CultureInfo.CurrentCulture;
            Calendar cal = ci.Calendar;
            CalendarWeekRule cwr = ci.DateTimeFormat.CalendarWeekRule;
            DayOfWeek firstDayOfWeek = ci.DateTimeFormat.FirstDayOfWeek;

            // �� ��¥�� ���� ���
            int weekOfYear1 = cal.GetWeekOfYear(srcDateTime, cwr, firstDayOfWeek);
            int weekOfYear2 = cal.GetWeekOfYear(dstDateTime, cwr, firstDayOfWeek);

            // ���� �ް� ���� �������� Ȯ��
            return srcDateTime.Month == dstDateTime.Month && weekOfYear1 == weekOfYear2;
        }

        public static bool IsToday(DateTime date) 
        {
            return date.Date == DateTime.Today;
        }
    }
}
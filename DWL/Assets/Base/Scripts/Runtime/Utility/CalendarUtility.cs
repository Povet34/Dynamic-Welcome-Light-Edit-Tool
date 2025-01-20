using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Utility
{
    public class CalendarUtility : MonoBehaviour
    {
        public class CellInfo
        {
            private int index;
            public int Index { get {  return index; } set { index = value; } }

            private DateTime date;
            public DateTime Date { get { return date; } set { date = value; } }
        }

        public const int TOTAL_DATE_NUM = 42; 
               
        public static (int, int, List<CellInfo>) GetCalendarData(DateTime dateTime)
        {
            DateTime firstDate = GetFirstDate(dateTime);
            int firstIndex = ConvertDayOfWeekToIndex(firstDate.DayOfWeek);

            List<CellInfo> cells = new List<CellInfo>();
            for (int index = 0; index < TOTAL_DATE_NUM; index++) 
            {
                int day = index - firstIndex;
                DateTime thatDate = firstDate.AddDays(day);
                CellInfo cellInfo = new CellInfo();
                cellInfo.Index = index;
                cellInfo.Date = thatDate;
                cells.Add(cellInfo);
            }

            return (dateTime.Year, dateTime.Month, cells);
        }

        public static DateTime GetFirstDate(DateTime dateTime)
        {
            return dateTime.AddDays(-(dateTime.Day - 1));
        }

        public static int ConvertDayOfWeekToIndex(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday    => 1,
                DayOfWeek.Tuesday   => 2,
                DayOfWeek.Wednesday => 3,
                DayOfWeek.Thursday  => 4,
                DayOfWeek.Friday    => 5,
                DayOfWeek.Saturday  => 6,
                DayOfWeek.Sunday    => 0,
                _ => -1,
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace YY.EventLogExportAssistant.ElasticSearch.Helpers
{
    public static class DateTimeHelpers
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime StartOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public static DateTime StartOfQuarter(this DateTime dt)
        {
            int quarterNumber = (dt.Month - 1) / 3 + 1;
            DateTime firstDayOfQuarter = new DateTime(dt.Year, (quarterNumber - 1) * 3 + 1, 1);
            return firstDayOfQuarter;
        }

        public static DateTime StartOfHalfYear(this DateTime dt)
        {
            DateTime start;

            var month = dt.Month;
            var year = dt.Year;
            if (month <= 6)
            {
                start = new DateTime(year, 1, 1);
            }
            else
            {
                start = new DateTime(year, 7, 1);
            }

            return start;
        }

        public static DateTime StartOfYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 1, 1);
        }
    }
}

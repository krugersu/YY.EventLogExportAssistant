using System;
using System.Collections.Generic;
using System.Text;

namespace YY.EventLogExportAssistant.ElasticSearch.Helpers
{
    public static class ElasticSearchHelper
    {
        public static string GetIndexSeparationPeriod(DateTime period, IndexSeparationPeriod separation)
        {
            string separationWithFormat;

            switch (separation)
            {
                case IndexSeparationPeriod.Hour:
                    separationWithFormat = period.ToString("yyyyMMddhh0000");
                    break;
                case IndexSeparationPeriod.Day:
                    separationWithFormat = period.ToString("yyyyMMdd000000");
                    break;
                case IndexSeparationPeriod.Week:
                    separationWithFormat = period.StartOfWeek(DayOfWeek.Monday).ToString("yyyyMMdd000000");
                    break;
                case IndexSeparationPeriod.Month:
                    separationWithFormat = period.StartOfMonth().ToString("yyyyMM01000000");
                    break;
                case IndexSeparationPeriod.Quarter:
                    separationWithFormat = period.StartOfQuarter().ToString("yyyyMM01000000");
                    break;
                case IndexSeparationPeriod.HalfYear:
                    separationWithFormat = period.StartOfHalfYear().ToString("yyyyMM01000000");
                    break;
                case IndexSeparationPeriod.Year:
                    separationWithFormat = period.StartOfYear().ToString("yyyyMM01000000");
                    break;
                default:
                    separationWithFormat = "FULL";
                    break;
            }

            return separationWithFormat;
        }
    }
}

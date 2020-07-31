using System;
using System.Linq;
using Nest;
using YY.EventLogExportAssistant.ElasticSearch.Models;

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

        public static LogFileElement GetLastLogFileElement(
            this ElasticClient client, 
            string systemName,
            string indexName)
        {
            string logFilesActualIndexName = $"{indexName}-LogFiles-Actual";
            logFilesActualIndexName = logFilesActualIndexName.ToLower();

            var searchResponse = client.Search<LogFileElement>(s => s
                .Index(logFilesActualIndexName)
                .From(0)
                .Size(1)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.InformationSystem)
                        .Query(systemName)
                    )
                )
            );

            if (!searchResponse.ApiCall.Success)
            {
                throw searchResponse.ApiCall.OriginalException;
            }

            if (searchResponse.Documents.Count == 1)
            {
                return searchResponse.Documents.First();
            }
            else
            {
                return null;
            }
        }
    }
}

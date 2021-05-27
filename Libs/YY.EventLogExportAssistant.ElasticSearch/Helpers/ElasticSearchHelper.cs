using System;
using System.Collections.Generic;
using System.Linq;
using Nest;
using YY.EventLogExportAssistant.ElasticSearch.Models;

namespace YY.EventLogExportAssistant.ElasticSearch.Helpers
{
    public static class ElasticSearchExtensions
    {
        public static string GetIndexSeparationPeriod(this DateTime period, IndexSeparationPeriod separation)
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
        public static string GetActualLogFilesIndexName(this string indexName)
        {
            return $"{indexName}-LogFiles-Actual".ToLower();
        }
        public static string GetHistoryLogFilesIndexName(this string indexName)
        {
            return $"{indexName}-LogFiles-History".ToLower();
        }
        public static LogFileElement GetLastLogFileElement(
            this ElasticClient client, 
            string systemName,
            string indexName)
        {
            string logFilesActualIndexName = GetActualLogFilesIndexName(indexName);

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
                throw searchResponse.ApiCall.OriginalException;

            if (searchResponse.Documents.Count == 1)
                return searchResponse.Documents.First();

            return null;
        }
        public static void SaveLogData(
            this ElasticClient client,
            List<LogDataElement> data,
            string logDataIndexName)
        {
            var indexManyResponse = client.IndexMany(data, logDataIndexName);

            if (!indexManyResponse.ApiCall.Success)
                throw indexManyResponse.ApiCall.OriginalException;

            if (indexManyResponse.IsValid == false || indexManyResponse.Errors)
            {
                string errorMessage =
                    $"При экспорте данных в индекс {logDataIndexName} произошли ошибки в " +
                    $"{indexManyResponse.ItemsWithErrors.Count()} из {data.Count} элементов.";
                throw new Exception(errorMessage);
            }
        }
        public static void SaveLogFileHistoryElement(
            this ElasticClient client,
            LogFileElement data,
            string indexName)
        {
            var indexHistoryResponse = client.Index(data, idx => idx.Index(indexName.GetHistoryLogFilesIndexName()));
            if (!indexHistoryResponse.ApiCall.Success)
                throw indexHistoryResponse.ApiCall.OriginalException;
        }
        public static void SaveLogFileActualElement(
            this ElasticClient client,
            LogFileElement data,
            string indexName)
        {
            var indexActualResponse = client.Index(data, idx => idx.Index(indexName.GetActualLogFilesIndexName()));
            if (!indexActualResponse.ApiCall.Success)
                throw indexActualResponse.ApiCall.OriginalException;
        }
    }
}

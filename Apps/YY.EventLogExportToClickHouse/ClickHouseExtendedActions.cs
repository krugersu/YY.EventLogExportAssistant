using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using YY.EventLogExportAssistant;
using YY.EventLogExportAssistant.ClickHouse;
using YY.EventLogExportAssistant.ClickHouse.Models;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportToClickHouse
{
    /// <summary>
    /// Пример реализации класса для переопределения событий инициализации базы данных и подготовки данных для сохранения.
    /// Добавлено поле "Дата документа", которое парсится из представления данных.
    /// </summary>
    public class ClickHouseExtendedActions : IExtendedActions
    {
        private static readonly DateTime _minDateTime = new DateTime(1970, 1, 1);
        private static readonly Regex _parseDateFromDataPresentation = new Regex(@"(\d|\d\d)\.(\d|\d\d)\.(\d\d\d\d) (\d|\d\d):(\d\d):(\d\d)");

        public void OnDatabaseModelConfiguring(object context, OnDatabaseModelConfiguringParameters parameters)
        {
            parameters.Query_CreateTable_RowsData = parameters.Query_CreateTable_RowsData.Replace(
                "{TemplateFields}",
                ",DocumentDate DateTime Codec(Delta, LZ4)");
        }
        public void BeforeSaveData(InformationSystemsBase system, List<RowData> rowsData, ref IEnumerable<object[]> dataToSave)
        {
            var dataToSaveNew = new List<object[]>();

            foreach (var objectItem in dataToSave)
            {
                string metadataName = objectItem[15].ToString();
                string dataPresentation = objectItem[19].ToString();
                DateTime documentDate = _minDateTime;
                if (metadataName != null && !string.IsNullOrEmpty(dataPresentation))
                {
                    if (metadataName.Contains("Документ", StringComparison.InvariantCultureIgnoreCase)
                        || metadataName.Contains("Document", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var matchResult = _parseDateFromDataPresentation.Match(dataPresentation);
                        DateTime.TryParseExact(
                            matchResult.Value, 
                            new[] { "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy H:mm:ss" }, 
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out documentDate);
                    }
                }

                dataToSaveNew.Add(new []
                {
                    objectItem[0],
                    objectItem[1],
                    objectItem[2],
                    objectItem[3],
                    objectItem[4],
                    objectItem[5],
                    objectItem[6],
                    objectItem[7],
                    objectItem[8],
                    objectItem[9],
                    objectItem[10],
                    objectItem[11],
                    objectItem[12],
                    objectItem[13],
                    objectItem[14],
                    metadataName,
                    objectItem[16],
                    objectItem[17],
                    objectItem[18],
                    dataPresentation,
                    objectItem[20],
                    objectItem[21],
                    objectItem[22],
                    (documentDate < _minDateTime ? _minDateTime : documentDate)
                });
            }

            dataToSave = dataToSaveNew;
        }
    }
}

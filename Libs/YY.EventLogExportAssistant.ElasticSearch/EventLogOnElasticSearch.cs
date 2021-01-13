using System.Collections.Generic;
using System.IO;
using YY.EventLogReaderAssistant;
using RowData = YY.EventLogReaderAssistant.Models.RowData;
using System;
using Microsoft.Extensions.Configuration;
using Nest;
using YY.EventLogExportAssistant.ElasticSearch.Helpers;
using YY.EventLogExportAssistant.ElasticSearch.Models;

namespace YY.EventLogExportAssistant.ElasticSearch
{
    public class EventLogOnElasticSearch : EventLogOnTarget
    {
        #region Private Member Variables

        private readonly ElasticClient _client;
        private string _indexName;
        private IndexSeparationPeriod _indexSeparationPeriod;

        #endregion

        #region Constructor

        public EventLogOnElasticSearch() : this(null, _defaultPortion)
        {

        }
        public EventLogOnElasticSearch(int portion) : this(null, portion)
        {
            _portion = portion;
        }
        public EventLogOnElasticSearch(ConnectionSettings elasticSettings, int portion)
        {
            _indexSeparationPeriod = IndexSeparationPeriod.None;
            _portion = portion;

            if (elasticSettings == null)
            {
                IConfiguration Configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
                IConfigurationSection elasticSearchSection = Configuration.GetSection("ElasticSearch");
                Uri nodeAddress = elasticSearchSection.GetValue<Uri>("Node");
                string indexName = elasticSearchSection.GetValue<string>("IndexName");
                string indexSeparation = elasticSearchSection.GetValue<string>("IndexSeparationPeriod");
                int maximumRetries = elasticSearchSection.GetValue<int>("MaximumRetries");
                int maxRetryTimeout = elasticSearchSection.GetValue<int>("MaxRetryTimeout");

                elasticSettings = new ConnectionSettings(nodeAddress)
                    .DefaultIndex(indexName)
                    .MaximumRetries(maximumRetries)
                    .MaxRetryTimeout(TimeSpan.FromSeconds(maxRetryTimeout));
                SetIndexSeparationPeriod(indexSeparation);
            }

            _client = new ElasticClient(elasticSettings);
        }

        #endregion

        #region Public Methods

        public override EventLogPosition GetLastPosition()
        {
            if (_lastEventLogFilePosition != null)
                return _lastEventLogFilePosition;

            LogFileElement actualLogFileInfo = _client.GetLastLogFileElement(_system.Name, _indexName);
            EventLogPosition position = null;
            if (actualLogFileInfo != null)
            {
                position = new EventLogPosition(
                    actualLogFileInfo.LastEventNumber,
                    actualLogFileInfo.LastCurrentFileReferences,
                    actualLogFileInfo.LastCurrentFileData,
                    actualLogFileInfo.LastStreamPosition
                );
            }
            _lastEventLogFilePosition = position;
            return position;
        }
        public override void SaveLogPosition(FileInfo logFileInfo, EventLogPosition position)
        {
            SaveLogFileHistoryElement(logFileInfo, position);
            SaveLogFileActualElement(logFileInfo, position);

            _lastEventLogFilePosition = position;
        }
        public override int GetPortionSize()
        {
            return _portion;
        }
        public override void Save(RowData rowData)
        {
            IList<RowData> rowsData = new List<RowData>
            {
                rowData
            };
            Save(rowsData);
        }
        public override void Save(IList<RowData> rowsData)
        {
            Dictionary<string, List<LogDataElement>> logDataByIndices = new Dictionary<string, List<LogDataElement>>();

            foreach (RowData item in rowsData)
            {
                string logDataCurrentIndexName = $"{ _indexName }-LogData-{ item.Period.GetIndexSeparationPeriod(_indexSeparationPeriod) }";
                logDataCurrentIndexName = logDataCurrentIndexName.ToLower();
                if (logDataByIndices.ContainsKey(logDataCurrentIndexName) == false)
                {
                    logDataByIndices.Add(logDataCurrentIndexName, new List<LogDataElement>());
                }

                logDataByIndices[logDataCurrentIndexName].Add(new LogDataElement(_system, item));
            }

            foreach (var indexItems in logDataByIndices)
                _client.SaveLogData(indexItems.Value, indexItems.Key);
            
        }
        public override void SetInformationSystem(InformationSystemsBase system)
        {
            _system = new InformationSystems()
            {
                Id = system.Id,
                Name = system.Name,
                Description = system.Description,
                TimeZoneName = system.TimeZoneName
            };

            if (_indexName == null)
            {
                _indexName = _system.Name;
            }
        }
        public void SetIndexName(string indexName)
        {
            _indexName = indexName;
        }
        public void SetIndexSeparationPeriod(string separation)
        {
            if (separation != null && Enum.TryParse(separation, true, out IndexSeparationPeriod separationValue))
            {
                SetIndexSeparationPeriod(separationValue);
            }
            else
            {
                SetIndexSeparationPeriod(IndexSeparationPeriod.None);
            }
        }
        public void SetIndexSeparationPeriod(IndexSeparationPeriod separation)
        {
            _indexSeparationPeriod = separation;
        }

        #endregion

        #region Private Methods

        private void SaveLogFileHistoryElement(FileInfo logFileInfo, EventLogPosition position)
        {
            _client.SaveLogFileHistoryElement(new LogFileElement()
            {
                Id = $"[{_system.Name}][{logFileInfo.Name}][{logFileInfo.CreationTimeUtc:yyyyMMddhhmmss}]",
                CreateDate = logFileInfo.CreationTimeUtc,
                FileName = logFileInfo.Name,
                InformationSystem = _system.Name,
                LastCurrentFileData = position.CurrentFileData,
                LastCurrentFileReferences = position.CurrentFileReferences,
                LastEventNumber = position.EventNumber,
                LastStreamPosition = position.StreamPosition,
                ModificationDate = logFileInfo.LastWriteTimeUtc
            }, _indexName);
        }
        private void SaveLogFileActualElement(FileInfo logFileInfo, EventLogPosition position)
        {
            _client.SaveLogFileActualElement(new LogFileElement()
            {
                Id = _system.Name,
                CreateDate = logFileInfo.CreationTimeUtc,
                FileName = logFileInfo.Name,
                InformationSystem = _system.Name,
                LastCurrentFileData = position.CurrentFileData,
                LastCurrentFileReferences = position.CurrentFileReferences,
                LastEventNumber = position.EventNumber,
                LastStreamPosition = position.StreamPosition,
                ModificationDate = logFileInfo.LastWriteTimeUtc
            }, _indexName);
        }

        #endregion
    }
}

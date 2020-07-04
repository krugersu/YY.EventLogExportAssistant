using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private const int _defaultPortion = 1000;
        private readonly int _portion;
        private readonly ElasticClient _client;
        private InformationSystemsBase _system;
        private string _indexName;
        private IndexSeparationPeriod _indexSeparationPeriod;
        private EventLogPosition _lastEventLogFilePosition;

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
            {
                return _lastEventLogFilePosition;
            }

            string logFilesActualIndexName = $"{ _indexName }-LogFiles-Actual";
            logFilesActualIndexName = logFilesActualIndexName.ToLower();

            var searchResponse = _client.Search<LogFileElement>(s => s
                .Index(logFilesActualIndexName)
                .From(0)
                .Size(1)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.InformationSystem)
                        .Query(_system.Name)
                    )
                )
            );

            EventLogPosition position = null;
            if (searchResponse.Documents.Count == 1)
            {
                LogFileElement actualLogFileInfo = searchResponse.Documents.First();

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
            LogFileElement logFileHistoryElement = new LogFileElement()
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
            };
            string logFilesHistoryIndexName = $"{ _indexName }-LogFiles-History";
            logFilesHistoryIndexName = logFilesHistoryIndexName.ToLower();
            _client.Index(logFileHistoryElement, idx => idx.Index(logFilesHistoryIndexName));

            LogFileElement logFilActualElement = new LogFileElement()
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
            };
            string logFilesActualIndexName = $"{ _indexName }-LogFiles-Actual";
            logFilesActualIndexName = logFilesActualIndexName.ToLower();
            _client.Index(logFilActualElement, idx => idx.Index(logFilesActualIndexName));

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
                string logDataCurrentIndexName = $"{ _indexName }-LogData-{ ElasticSearchHelper.GetIndexSeparationPeriod(item.Period.DateTime, _indexSeparationPeriod) }";
                logDataCurrentIndexName = logDataCurrentIndexName.ToLower();
                if (logDataByIndices.ContainsKey(logDataCurrentIndexName) == false)
                {
                    logDataByIndices.Add(logDataCurrentIndexName, new List<LogDataElement>());
                }

                logDataByIndices[logDataCurrentIndexName].Add(new LogDataElement()
                {
                    Id = $"[{_system.Name}][{item.Period:yyyyMMddhhmmss}][{item.RowId}]",
                    Application = item.Application.Name,
                    Comment = item.Comment,
                    Computer = item.Computer?.Name,
                    ConnectionId = item.ConnectId,
                    Data = item.Data,
                    DataPresentation = item.DataPresentation,
                    DataUUID = item.DataUuid,
                    Event = item.Event?.Name,
                    RowId = item.RowId,
                    InformationSystem = _system.Name,
                    Metadata = item.Metadata?.Name,
                    MetadataUUID = item.Metadata?.Uuid.ToString(),
                    Period = item.Period,
                    PrimaryPort = item.PrimaryPort?.Name,
                    SecondaryPort = item.SecondaryPort?.Name,
                    Session = item.Session,
                    Severity = item.Severity.ToString(),
                    TransactionDate = item.TransactionDate,
                    TransactionId = item.TransactionId,
                    TransactionStatus = item.TransactionStatus.ToString(),
                    User = item.User?.Name,
                    UserUUID = item.User?.Uuid.ToString(),
                    WorkServer = item.WorkServer?.Name
                });
            }

            foreach (var indexItems in logDataByIndices)
            {
                string logDataIndexName = indexItems.Key;
                var indexManyResponse = _client.IndexMany(indexItems.Value, logDataIndexName);

                if (indexManyResponse.IsValid == false || indexManyResponse.Errors)
                {
                    string errorMessage =
                        $"При экспорте данных в индекс {logDataIndexName} произошли ошибки в {indexManyResponse.ItemsWithErrors.Count()} из {indexItems.Value.Count} элементов.";
                    throw new Exception(errorMessage);
                }
            }
        }
        public override void SetInformationSystem(InformationSystemsBase system)
        {
            _system = new InformationSystems()
            {
                Id = system.Id,
                Name = system.Name,
                Description = system.Description
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
                SetIndexSaparationPeriod(separationValue);
            }
            else
            {
                SetIndexSaparationPeriod(IndexSeparationPeriod.None);
            }
        }
        public void SetIndexSaparationPeriod(IndexSeparationPeriod separation)
        {
            _indexSeparationPeriod = separation;
        }
        public override void UpdateReferences(ReferencesData data)
        {
        }

        #endregion
    }
}

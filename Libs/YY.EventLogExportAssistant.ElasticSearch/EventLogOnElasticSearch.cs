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
using YY.EventLogReaderAssistant;

namespace YY.EventLogExportAssistant.ElasticSearch
{
    public class EventLogOnElasticSearch : EventLogOnTarget
    {
        #region Private Member Variables

        private const int _defaultPortion = 1000;
        private readonly int _portion;
        private readonly ConnectionSettings _elasticSettings;
        private readonly ElasticClient _client;
        private InformationSystemsBase _system;
        private string _indexName;
        private IndexSeparationPeriod _indexSeparationPeriod;
        private DateTime _maxPeriodRowData;

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
            _maxPeriodRowData = DateTime.MinValue;
            _portion = portion;

            if (elasticSettings == null)
            {
                IConfiguration Configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
                IConfigurationSection elasticSearchSection = Configuration.GetSection("ElasticSearch");
                Uri nodeAddress = elasticSearchSection.GetValue<Uri>("Node");
                string indexName = elasticSearchSection.GetValue<string>("IndexName");
                int maximumRetries = elasticSearchSection.GetValue<int>("MaximumRetries");
                int maxRetryTimeout = elasticSearchSection.GetValue<int>("MaxRetryTimeout");

                _elasticSettings = new ConnectionSettings(nodeAddress)
                    .DefaultIndex(indexName)
                    .MaximumRetries(maximumRetries)
                    .MaxRetryTimeout(TimeSpan.FromSeconds(maxRetryTimeout));
            }
            else
            {
                _elasticSettings = elasticSettings;
            }

            _client = new ElasticClient(_elasticSettings);
        }

        #endregion

        #region Public Methods

        public override EventLogPosition GetLastPosition()
        {
            // TODO: Read last position from Elastic Index
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
            List<LogDataElement> items = new List<LogDataElement>();
            foreach (RowData item in rowsData)
            {
                items.Add(new LogDataElement()
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

            string logDataIndexName = $"{ _indexName }-LogData-{ ElasticSearchHelper.GetIndexSeparationPeriod(DateTime.Now, _indexSeparationPeriod) }";
            logDataIndexName = logDataIndexName.ToLower();
            var indexManyResponse = _client.IndexMany(items, logDataIndexName);
            
            // TODO: Do actions on errors
            //if (indexManyResponse.Errors)
            //{
            //}
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

            // TODO: Create empty index for data if not exists
            // TODO: Create empty index for status last log's files
            // OR NOT???
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
            // TODO:
            // There is no necessary to do something here, because data of reference put inside the index
        }

        #endregion
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using YY.EventLogReaderAssistant;
using RowData = YY.EventLogReaderAssistant.Models.RowData;
using System;
using Microsoft.Extensions.Configuration;
using Nest;
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
            // TODO: Read last position info from Elastic Index

            throw new NotImplementedException();
        }
        public override void SaveLogPosition(FileInfo logFileInfo, EventLogPosition position)
        {
            // TODO: Save last position info to Elastic Index
            // with model LogFileElement

            var logFileElement = new LogFileElement()
            {
                CreateDate = logFileInfo.CreationTimeUtc,
                FileName = logFileInfo.Name,
                InformationSystem = _system.Name,
                LastCurrentFileData = position.CurrentFileData,
                LastCurrentFileReferences = position.CurrentFileReferences,
                LastEventNumber = position.EventNumber,
                LastStreamPosition = position.StreamPosition,
                ModificationDate = logFileInfo.LastWriteTimeUtc
            };

            // TODO: Index for log file elements
            //_client.Index(logFileElement, idx => idx.Index("IS-LogFiles"));
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
            // TODO: Convert RowData to LogDataElement and add to index

            List<LogDataElement> items = new List<LogDataElement>();
            foreach (RowData item in rowsData)
            {
                items.Add(new LogDataElement()
                {
                    Id = item.RowId,
                    Application = item.Application.Name,
                    Comment = item.Comment,
                    Computer = item.Computer.Name,
                    ConnectionId = item.ConnectId,
                    Data = item.Data,
                    DataPresentation = item.DataPresentation,
                    DataUUID = item.DataUuid,
                    Event = item.Event.Name,
                    InformationSystem = _system.Name,
                    Metadata = item.Metadata.Name,
                    MetadataUUID = item.Metadata.Uuid.ToString(),
                    Period = item.Period,
                    PrimaryPort = item.PrimaryPort.Name,
                    SecondaryPort = item.SecondaryPort.Name,
                    Session = item.Session,
                    Severity = item.Severity.ToString(),
                    TransactionDate = item.TransactionDate,
                    TransactionId = item.TransactionId,
                    TransactionStatus = item.TransactionStatus.ToString(),
                    User = item.User.Name,
                    UserUUID = item.User.Uuid.ToString(),
                    WorkServer = item.WorkServer.Name
                });
            }

            // TODO: Bulk index for log data elements
            //_client.IndexMany(items, idx => idx.Index("IS-LogData"));
        }
        public override void SetInformationSystem(InformationSystemsBase system)
        {
            _system = new InformationSystems()
            {
                Id = system.Id,
                Name = system.Name,
                Description = system.Description
            };

            // TODO: Create empty index for data if not exists
            // TODO: Create empty index for status last log's files
            //_client.Index(_system, idx => idx.Index("event-log"));
        }
        public override void UpdateReferences(ReferencesData data)
        {
            // TODO:
            // There is no necessary to do something here, because data of reference put inside the index
        }

        #endregion
    }
}

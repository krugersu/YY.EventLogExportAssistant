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
        private InformationSystemsBase _system;
        private DateTime _maxPeriodRowData;

        private IReadOnlyList<Applications> cacheApplications;
        private IReadOnlyList<Computers> cacheComputers;
        private IReadOnlyList<Events> cacheEvents;
        private IReadOnlyList<Metadata> cacheMetadata;
        private IReadOnlyList<PrimaryPorts> cachePrimaryPorts;
        private IReadOnlyList<SecondaryPorts> cacheSecondaryPorts;
        private IReadOnlyList<Severities> cacheSeverities;
        private IReadOnlyList<TransactionStatuses> cacheTransactionStatuses;
        private IReadOnlyList<Users> cacheUsers;
        private IReadOnlyList<WorkServers> cacheWorkServers;

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
                _elasticSettings = elasticSettings;
        }

        #endregion

        #region Public Methods

        public override EventLogPosition GetLastPosition()
        {
            throw new NotImplementedException();
        }
        public override void SaveLogPosition(FileInfo logFileInfo, EventLogPosition position)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
        public override void SetInformationSystem(InformationSystemsBase system)
        {
            _system = new InformationSystems()
            {
                Id = system.Id,
                Name = system.Name,
                Description = system.Description
            };

            ElasticClient client = new ElasticClient(_elasticSettings);
            client.IndexDocument(_system);
        }
        public override void UpdateReferences(ReferencesData data)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

using System.Collections.Generic;
using System.IO;
using YY.EventLogReaderAssistant;
using RowData = YY.EventLogReaderAssistant.Models.RowData;
using System;
using ClickHouse.Ado;
using Microsoft.Extensions.Configuration;

namespace YY.EventLogExportAssistant.ClickHouse
{
    public class EventLogOnClickHouse : EventLogOnTarget
    {
        #region Private Member Variables

        private const int _defaultPortion = 1000;
        private readonly int _portion;
        private readonly ClickHouseContext _context;
        private InformationSystemsBase _system;
        private EventLogPosition _lastEventLogFilePosition;

        #endregion

        #region Constructor

        public EventLogOnClickHouse() : this(null, _defaultPortion)
        {

        }
        public EventLogOnClickHouse(int portion) : this(null, portion)
        {
            _portion = portion;
        }
        public EventLogOnClickHouse(ClickHouseConnectionSettings clickHouseSettings, int portion)
        {
            _portion = portion;

            if (clickHouseSettings == null)
            {
                IConfiguration Configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
                string connectionString = Configuration.GetConnectionString("EventLogDatabase");

                clickHouseSettings = new ClickHouseConnectionSettings(connectionString);
            }

            _context = new ClickHouseContext(clickHouseSettings);
        }

        #endregion

        #region Public Methods

        public override EventLogPosition GetLastPosition()
        {
            if (_lastEventLogFilePosition != null)
                return _lastEventLogFilePosition;

            EventLogPosition position = _context.GetLogFilePosition(_system.Id);
            
            _lastEventLogFilePosition = position;
            return position;
        }
        public override void SaveLogPosition(FileInfo logFileInfo, EventLogPosition position)
        {
            _context.SaveLogPosition(_system, logFileInfo, position);

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
            

            //Dictionary<string, List<LogDataElement>> logDataByIndices = new Dictionary<string, List<LogDataElement>>();

            //foreach (RowData item in rowsData)
            //{
            //    string logDataCurrentIndexName = $"{ _indexName }-LogData-{ item.Period.DateTime.GetIndexSeparationPeriod(_indexSeparationPeriod) }";
            //    logDataCurrentIndexName = logDataCurrentIndexName.ToLower();
            //    if (logDataByIndices.ContainsKey(logDataCurrentIndexName) == false)
            //    {
            //        logDataByIndices.Add(logDataCurrentIndexName, new List<LogDataElement>());
            //    }

            //    logDataByIndices[logDataCurrentIndexName].Add(new LogDataElement(_system, item));
            //}

            //foreach (var indexItems in logDataByIndices)
            //    _client.SaveLogData(indexItems.Value, indexItems.Key);
            
        }
        public override void SetInformationSystem(InformationSystemsBase system)
        {
            _system = _context.CreateOrUpdateInformationSystem(system.Name, system.Description);
        }
        public override void UpdateReferences(ReferencesData data)
        {
            foreach (var item in data.Applications)
                _context.AddApplicationIfNotExist(_system.Id, item);

            foreach (var item in data.Computers)
                _context.AddComputerIfNotExist(_system.Id, item);

            foreach (var item in data.Events)
                _context.AddEventIfNotExist(_system.Id, item);

            foreach (var item in data.Metadata)
                _context.AddMetadataIfNotExist(_system.Id, item);

            foreach (var item in data.PrimaryPorts)
                _context.AddPrimaryPortIfNotExist(_system.Id, item);

            foreach (var item in data.SecondaryPorts)
                _context.AddSecondaryPortIfNotExist(_system.Id, item);

            foreach (var item in data.Severities)
                _context.AddSeverityIfNotExist(_system.Id, item);

            foreach (var item in data.TransactionStatuses)
                _context.AddTransactionStatusIfNotExist(_system.Id, item);

            foreach (var item in data.Users)
                _context.AddUserIfNotExist(_system.Id, item);

            foreach (var item in data.WorkServers)
                _context.AddWorkServerIfNotExist(_system.Id, item);
        }

        #endregion

        #region Private Methods
        


        #endregion
    }
}

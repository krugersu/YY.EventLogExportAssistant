using System.Collections.Generic;
using System.IO;
using YY.EventLogReaderAssistant;
using RowData = YY.EventLogReaderAssistant.Models.RowData;
using System;
using ClickHouse.Ado;
using Microsoft.Extensions.Configuration;
using YY.EventLogExportAssistant.ClickHouse.Models;
using YY.EventLogExportAssistant.Database;

namespace YY.EventLogExportAssistant.ClickHouse
{
    public class EventLogOnClickHouse : EventLogOnTarget
    {
        #region Private Member Variables

        private const int _defaultPortion = 1000;
        private readonly int _portion;
        private DateTime _maxPeriodRowData;
        private InformationSystemsBase _system;
        private readonly string _connectionString;
        private EventLogPosition _lastEventLogFilePosition;
        private ReferencesDataCache _referencesCache;

        #endregion

        #region Constructor

        public EventLogOnClickHouse() : this(null, _defaultPortion)
        {

        }
        public EventLogOnClickHouse(int portion) : this(null, portion)
        {
            _portion = portion;
        }
        public EventLogOnClickHouse(string connectionString, int portion)
        {
            _portion = portion;
            _maxPeriodRowData = DateTime.MinValue;

            if (connectionString == null)
            {
                IConfiguration Configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
                _connectionString = Configuration.GetConnectionString("EventLogDatabase");
            }

            _connectionString = connectionString;
        }

        #endregion

        #region Public Methods

        public override EventLogPosition GetLastPosition()
        {
            if (_lastEventLogFilePosition != null)
                return _lastEventLogFilePosition;

            EventLogPosition position;
            using(var context = new ClickHouseContext(_connectionString))
                position = context.GetLogFilePosition(_system.Id);
            
            _lastEventLogFilePosition = position;
            return position;
        }
        public override void SaveLogPosition(FileInfo logFileInfo, EventLogPosition position)
        {
            using (var context = new ClickHouseContext(_connectionString))
                context.SaveLogPosition(_system, logFileInfo, position);

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
            using (var context = new ClickHouseContext(_connectionString))
            {
                if (_maxPeriodRowData == DateTime.MinValue)
                    _maxPeriodRowData = context.GetRowsDataMaxPeriod(_system);

                List<RowDataBulkInsert> newEntities = new List<RowDataBulkInsert>();
                foreach (var itemRow in rowsData)
                {
                    if (itemRow == null)
                        continue;
                    if (_maxPeriodRowData != DateTime.MinValue && itemRow.Period.DateTime <= _maxPeriodRowData)
                        if (context.RowDataExistOnDatabase(_system, itemRow))
                            continue;

                    newEntities.Add(new RowDataBulkInsert(_system, itemRow, _referencesCache));
                }

                context.SaveRowsData(newEntities);
            }
        }
        public override void SetInformationSystem(InformationSystemsBase system)
        {
            using (var context = new ClickHouseContext(_connectionString))
                _system = context.CreateOrUpdateInformationSystem(system.Name, system.Description);
        }
        public override void UpdateReferences(ReferencesData data)
        {
            using (var context = new ClickHouseContext(_connectionString))
            {
                foreach (var item in data.Applications)
                    context.AddApplicationIfNotExist(_system.Id, item);

                foreach (var item in data.Computers)
                    context.AddComputerIfNotExist(_system.Id, item);

                foreach (var item in data.Events)
                    context.AddEventIfNotExist(_system.Id, item);

                foreach (var item in data.Metadata)
                    context.AddMetadataIfNotExist(_system.Id, item);

                foreach (var item in data.PrimaryPorts)
                    context.AddPrimaryPortIfNotExist(_system.Id, item);

                foreach (var item in data.SecondaryPorts)
                    context.AddSecondaryPortIfNotExist(_system.Id, item);

                foreach (var item in data.Severities)
                    context.AddSeverityIfNotExist(_system.Id, item);

                foreach (var item in data.TransactionStatuses)
                    context.AddTransactionStatusIfNotExist(_system.Id, item);

                foreach (var item in data.Users)
                    context.AddUserIfNotExist(_system.Id, item);

                foreach (var item in data.WorkServers)
                    context.AddWorkServerIfNotExist(_system.Id, item);

                if (_referencesCache == null)
                    _referencesCache = new ReferencesDataCache(_system);
                context.FillReferencesCacheByDatabaseContext(_system.Id, _referencesCache);
            }
        }

        #endregion
    }
}

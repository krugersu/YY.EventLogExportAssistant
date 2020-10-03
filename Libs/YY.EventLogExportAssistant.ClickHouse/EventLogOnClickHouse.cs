using System.Collections.Generic;
using System.IO;
using YY.EventLogReaderAssistant;
using RowData = YY.EventLogReaderAssistant.Models.RowData;
using System;
using Microsoft.Extensions.Configuration;

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
                position = context.GetLogFilePosition(_system);
            
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

                List<RowData> newEntities = new List<RowData>();
                foreach (var itemRow in rowsData)
                {
                    if (itemRow == null)
                        continue;
                    if (_maxPeriodRowData != DateTime.MinValue && itemRow.Period.DateTime <= _maxPeriodRowData)
                        if (context.RowDataExistOnDatabase(_system, itemRow))
                            continue;

                    newEntities.Add(itemRow);
                }

                context.SaveRowsData(_system, newEntities);
            }
        }
        public override void SetInformationSystem(InformationSystemsBase system)
        {
            _system = system;
        }
        public override void UpdateReferences(ReferencesData data)
        {
        }

        #endregion
    }
}

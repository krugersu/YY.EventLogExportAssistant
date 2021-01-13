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

        private readonly string _connectionString;
        private int _stepsToClearLogFiles = 1000;
        private int _currentStepToClearLogFiles;
        private readonly IExtendedActions _extendedActions;

        #endregion

        #region Constructor

        public EventLogOnClickHouse() : this(null, _defaultPortion, null)
        {
        }
        public EventLogOnClickHouse(int portion) : this(null, portion, null)
        {
        }
        public EventLogOnClickHouse(string connectionString, int portion) : this(connectionString, portion, null)
        {
        }
        public EventLogOnClickHouse(string connectionString, int portion, IExtendedActions extendedActions)
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
            _extendedActions = extendedActions;
        }

        #endregion

        #region Public Methods

        public override EventLogPosition GetLastPosition()
        {
            if (_lastEventLogFilePosition != null)
                return _lastEventLogFilePosition;

            EventLogPosition position;
            using(var context = new ClickHouseContext(_connectionString, _extendedActions))
                position = context.GetLogFilePosition(_system);
            
            _lastEventLogFilePosition = position;
            return position;
        }
        public override void SaveLogPosition(FileInfo logFileInfo, EventLogPosition position)
        {
            using (var context = new ClickHouseContext(_connectionString, _extendedActions))
            {
                context.SaveLogPosition(_system, logFileInfo, position);
                if (_currentStepToClearLogFiles == 0 || _currentStepToClearLogFiles >= _stepsToClearLogFiles)
                {
                    context.RemoveArchiveLogFileRecords(_system);
                    _currentStepToClearLogFiles = 0;
                }
                _currentStepToClearLogFiles += 1;
            }

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
            using (var context = new ClickHouseContext(_connectionString, _extendedActions))
            {
                if (_maxPeriodRowData == DateTime.MinValue)
                    _maxPeriodRowData = context.GetRowsDataMaxPeriod(_system);

                List<RowData> newEntities = new List<RowData>();
                foreach (var itemRow in rowsData)
                {
                    if (itemRow == null)
                        continue;
                    if (_maxPeriodRowData != DateTime.MinValue && itemRow.Period <= _maxPeriodRowData)
                        if (context.RowDataExistOnDatabase(_system, itemRow))
                            continue;

                    newEntities.Add(itemRow);
                }
                context.SaveRowsData(_system, newEntities);
            }
        }

        #endregion
    }
}

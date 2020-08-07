using System.Collections.Generic;
using System.IO;
using YY.EventLogReaderAssistant;
using RowData = YY.EventLogReaderAssistant.Models.RowData;
using Microsoft.EntityFrameworkCore;
using System;
using YY.EventLogExportAssistant.Database;

namespace YY.EventLogExportAssistant.MySQL
{
    public class EventLogOnMySQL : EventLogOnTarget
    {
        #region Private Member Variables

        private const int _defaultPortion = 1000;
        private readonly int _portion;
        private readonly DbContextOptions<EventLogContext> _databaseOptions;
        private InformationSystemsBase _system;
        private DateTime _maxPeriodRowData;
        private readonly IEventLogContextExtensionActions _mySqlActions;
        private ReferencesDataCache _referencesCache;

        #endregion

        #region Constructor

        public EventLogOnMySQL() : this(null, _defaultPortion)
        {
            
        }
        public EventLogOnMySQL(int portion) : this(null, portion)
        {
            _portion = portion;
        }
        public EventLogOnMySQL(DbContextOptions<EventLogContext> databaseOptions, int portion)
        {
            _mySqlActions = new EventLogMySQLActions();
            _maxPeriodRowData = DateTime.MinValue;
            _portion = portion;
            if (databaseOptions == null)
            {
                var optionsBuilder = new DbContextOptionsBuilder<EventLogContext>();
                _mySqlActions.OnConfiguring(optionsBuilder);
                _databaseOptions = optionsBuilder.Options;
            }
            else
                _databaseOptions = databaseOptions;
        }

        #endregion

        #region Public Methods

        public override EventLogPosition GetLastPosition()
        {
            using (EventLogContext _context = EventLogContext.Create(_databaseOptions, _mySqlActions))
                return _context.GetLastPosition(_system);
        }
        public override void SaveLogPosition(FileInfo logFileInfo, EventLogPosition position)
        {
            using (EventLogContext _context = EventLogContext.Create(_databaseOptions, _mySqlActions))
                _context.SaveLogPosition(_system, logFileInfo, position);
        }
        public override int GetPortionSize()
        {
            return _portion;
        }
        public override void Save(RowData rowData)
        {
            Save(new List<RowData>
            {
                rowData
            });
        }
        public override void Save(IList<RowData> rowsData)
        {
            using (EventLogContext _context = EventLogContext.Create(_databaseOptions, _mySqlActions))
            {
                if (_maxPeriodRowData == DateTime.MinValue)
                    _maxPeriodRowData = _context.GetRowsDataMaxPeriod(_system);

                List<Database.Models.RowData> newEntities = new List<Database.Models.RowData>();
                foreach (var itemRow in rowsData)
                {
                    if (itemRow == null)
                        continue;
                    if (_maxPeriodRowData != DateTime.MinValue && itemRow.Period <= _maxPeriodRowData)
                        if (_context.RowDataExistOnDatabase(_system, itemRow))
                            continue;

                    newEntities.Add(new Database.Models.RowData(_system, itemRow, _referencesCache));
                }
                
                _context.AddRange(newEntities);
                _context.SaveChanges();
            }
        }
        public override void SetInformationSystem(InformationSystemsBase system)
        {
            using (EventLogContext _context = EventLogContext.Create(_databaseOptions, _mySqlActions))
                _system = _context.CreateOrUpdateInformationSystem(system);
        }
        public override void UpdateReferences(ReferencesData data)
        {
            using (EventLogContext _context = EventLogContext.Create(_databaseOptions, _mySqlActions))
            {
                _context.FillReferencesToSave(_system, data);
                _context.SaveChanges();

                if (_referencesCache == null)
                    _referencesCache = new ReferencesDataCache(_system);
                _referencesCache.FillByDatabaseContext(_context);
            }
        }

        #endregion
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using YY.EventLogReaderAssistant;
using RowData = YY.EventLogReaderAssistant.Models.RowData;
using Microsoft.EntityFrameworkCore;
using Npgsql.Bulk;
using System;
using YY.EventLogExportAssistant.Database;
using YY.EventLogExportAssistant.Database.Models;

namespace YY.EventLogExportAssistant.PostgreSQL
{
    public class EventLogOnPostgreSQL : EventLogOnTarget
    {
        #region Private Member Variables

        private const int _defaultPortion = 1000;
        private readonly int _portion;
        private readonly DbContextOptions<EventLogContext> _databaseOptions;
        private InformationSystemsBase _system;
        private DateTime _maxPeriodRowData;
        private readonly IEventLogContextExtensionActions _postgreSqlActions;

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

        public EventLogOnPostgreSQL() : this(null, _defaultPortion)
        {

        }
        public EventLogOnPostgreSQL(int portion) : this(null, portion)
        {
            _portion = portion;
        }
        public EventLogOnPostgreSQL(DbContextOptions<EventLogContext> databaseOptions, int portion)
        {
            _postgreSqlActions = new EventLogPostgreSQLActions();
            _maxPeriodRowData = DateTime.MinValue;
            _portion = portion;
            if (databaseOptions == null)
            {
                var optionsBuilder = new DbContextOptionsBuilder<EventLogContext>();
                _postgreSqlActions.OnConfiguring(optionsBuilder);
                _databaseOptions = optionsBuilder.Options;
            }
            else
                _databaseOptions = databaseOptions;
        }

        #endregion

        #region Public Methods

        public override EventLogPosition GetLastPosition()
        {
            using (EventLogContext _context = EventLogContext.Create(_databaseOptions, _postgreSqlActions, DBMSType.PostgreSQL))
            {
                var lastLogFile = _context.LogFiles
                    .SingleOrDefault(e => e.InformationSystemId == _system.Id
                                          && e.Id == _context.LogFiles.Where(i => i.InformationSystemId == _system.Id).Max(m => m.Id));

                if (lastLogFile == null)
                    return null;
                else
                    return new EventLogPosition(
                        lastLogFile.LastEventNumber,
                        lastLogFile.LastCurrentFileReferences,
                        lastLogFile.LastCurrentFileData,
                        lastLogFile.LastStreamPosition);
            }
        }
        public override void SaveLogPosition(FileInfo logFileInfo, EventLogPosition position)
        {
            using (EventLogContext _context = EventLogContext.Create(_databaseOptions, _postgreSqlActions, DBMSType.PostgreSQL))
            {
                LogFiles foundLogFile = _context.LogFiles
                    .FirstOrDefault(l => l.InformationSystemId == _system.Id && l.FileName == logFileInfo.Name && l.CreateDate == logFileInfo.CreationTimeUtc);
                
                if (foundLogFile == null)
                {
                    _context.LogFiles.Add(new LogFiles()
                    {
                        InformationSystemId = _system.Id,
                        FileName = logFileInfo.Name,
                        CreateDate = logFileInfo.CreationTimeUtc,
                        ModificationDate = logFileInfo.LastWriteTimeUtc,
                        LastCurrentFileData = position.CurrentFileData,
                        LastCurrentFileReferences = position.CurrentFileReferences,
                        LastEventNumber = position.EventNumber,
                        LastStreamPosition = position.StreamPosition
                    });
                }
                else
                {
                    foundLogFile.ModificationDate = logFileInfo.LastWriteTimeUtc;
                    foundLogFile.LastCurrentFileData = position.CurrentFileData;
                    foundLogFile.LastCurrentFileReferences = position.CurrentFileReferences;
                    foundLogFile.LastEventNumber = position.EventNumber;
                    foundLogFile.LastStreamPosition = position.StreamPosition;
                    _context.Entry(foundLogFile).State = EntityState.Modified;
                }

                _context.SaveChanges();
            }
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
            using (EventLogContext _context = EventLogContext.Create(_databaseOptions, _postgreSqlActions, DBMSType.PostgreSQL))
            {
                if (_maxPeriodRowData == DateTime.MinValue)
                    _maxPeriodRowData = _context.GetRowsDataMaxPeriod(_system);

                List<Database.Models.RowData> newEntities = new List<Database.Models.RowData>();
                foreach (var itemRow in rowsData)
                {
                    if (itemRow == null)
                        continue;
                    if(_maxPeriodRowData != DateTime.MinValue && itemRow.Period <= _maxPeriodRowData)
                    {
                        var checkExist = _context.RowsData
                            .FirstOrDefault(e => e.InformationSystemId == _system.Id && e.Period == itemRow.Period && e.Id == itemRow.RowId);
                        if (checkExist != null)
                            continue;
                    }

                    long? rowApplicationId = null;
                    if (itemRow.Application != null)
                    {
                        var rowApplication = cacheApplications
                            .First(e => e.InformationSystemId == _system.Id && e.Name == itemRow.Application.Name);
                        rowApplicationId = rowApplication.Id;
                    }

                    long? rowComputerId = null;
                    if (itemRow.Computer != null)
                    {
                        var rowComputer = cacheComputers
                            .First(e => e.InformationSystemId == _system.Id && e.Name == itemRow.Computer.Name);
                        rowComputerId = rowComputer.Id;
                    }

                    long? rowEventId = null;
                    if (itemRow.Event != null)
                    {
                        var rowEvent = cacheEvents
                            .First(e => e.InformationSystemId == _system.Id && e.Name == itemRow.Event.Name);
                        rowEventId = rowEvent.Id;
                    }

                    long? rowMetadataId = null;
                    if (itemRow.Metadata != null)
                    {
                        var rowMetadata = cacheMetadata
                            .First(e => e.InformationSystemId == _system.Id && e.Name == itemRow.Metadata.Name && e.Uuid == itemRow.Metadata.Uuid);
                        rowMetadataId = rowMetadata.Id;
                    }

                    long? rowPrimaryPortId = null;
                    if (itemRow.PrimaryPort != null)
                    {
                        var rowPrimaryPort = cachePrimaryPorts
                            .First(e => e.InformationSystemId == _system.Id && e.Name == itemRow.PrimaryPort.Name);
                        rowPrimaryPortId = rowPrimaryPort.Id;
                    }

                    long? rowSecondaryPortId = null;
                    if (itemRow.SecondaryPort != null)
                    {
                        var rowSecondaryPort = cacheSecondaryPorts
                            .First(e => e.InformationSystemId == _system.Id && e.Name == itemRow.SecondaryPort.Name);
                        rowSecondaryPortId = rowSecondaryPort.Id;
                    }

                    var rowSeverity = cacheSeverities
                        .First(e => e.InformationSystemId == _system.Id && e.Name == itemRow.Severity.ToString());
                    var rowTransactionStatus = cacheTransactionStatuses
                        .First(e => e.InformationSystemId == _system.Id && e.Name == itemRow.TransactionStatus.ToString());

                    long? rowUserId = null;
                    if (itemRow.User != null)
                    {
                        var rowUser = cacheUsers
                            .First(e => e.InformationSystemId == _system.Id && e.Name == itemRow.User.Name);
                        rowUserId = rowUser.Id;
                    }

                    long? rowWorkServerId = null;
                    if (itemRow.WorkServer != null)
                    {
                        var rowWorkServer = cacheWorkServers
                            .First(e => e.InformationSystemId == _system.Id && e.Name == itemRow.WorkServer.Name);
                        rowWorkServerId = rowWorkServer.Id;
                    }

                    Database.Models.RowData rowData = new Database.Models.RowData()
                    {
                        ApplicationId = rowApplicationId,
                        Comment = itemRow.Comment,
                        ComputerId = rowComputerId,
                        ConnectId = itemRow.ConnectId,
                        Data = itemRow.Data,
                        DataPresentation = itemRow.DataPresentation,
                        DataUUID = itemRow.DataUuid,
                        EventId = rowEventId,
                        Id = itemRow.RowId,
                        InformationSystemId = _system.Id,
                        MetadataId = rowMetadataId,
                        Period = itemRow.Period,
                        PrimaryPortId = rowPrimaryPortId,
                        SecondaryPortId = rowSecondaryPortId,
                        Session = itemRow.Session,
                        SeverityId = rowSeverity.Id,
                        TransactionDate = itemRow.TransactionDate,
                        TransactionId = itemRow.TransactionId,
                        TransactionStatusId = rowTransactionStatus.Id,
                        UserId = rowUserId,
                        WorkServerId = rowWorkServerId
                    };

                    newEntities.Add(rowData);
                }

                var bulkUploader = new NpgsqlBulkUploader(_context);
                bulkUploader.Insert(newEntities);
            }
        }
        public override void SetInformationSystem(InformationSystemsBase system)
        {
            using (EventLogContext _context = EventLogContext.Create(_databaseOptions, _postgreSqlActions, DBMSType.PostgreSQL))
            {
                InformationSystems existSystem = _context.InformationSystems.FirstOrDefault(e => e.Name == system.Name);
                if (existSystem == null)
                {
                    _context.InformationSystems.Add(new InformationSystems()
                    {
                        Name = system.Name,
                        Description = system.Description
                    });
                    _context.SaveChanges();
                    existSystem = _context.InformationSystems.FirstOrDefault(e => e.Name == system.Name);
                }
                else
                {
                    if (existSystem.Description != system.Description)
                    {
                        existSystem.Description = system.Description;
                        _context.Update(system);
                        _context.SaveChanges();
                    }
                }

                _system = existSystem;
            }
        }
        public override void UpdateReferences(ReferencesData data)
        {
            using (EventLogContext _context = EventLogContext.Create(_databaseOptions, _postgreSqlActions, DBMSType.PostgreSQL))
            {
                _context.FillReferencesToSave(_system, data);
                _context.SaveChanges();

                cacheApplications = _context.Applications.ToList().AsReadOnly();
                cacheComputers = _context.Computers.ToList().AsReadOnly();
                cacheEvents = _context.Events.ToList().AsReadOnly();
                cacheMetadata = _context.Metadata.ToList().AsReadOnly();
                cachePrimaryPorts = _context.PrimaryPorts.ToList().AsReadOnly();
                cacheSecondaryPorts = _context.SecondaryPorts.ToList().AsReadOnly();
                cacheSeverities = _context.Severities.ToList().AsReadOnly();
                cacheTransactionStatuses = _context.TransactionStatuses.ToList().AsReadOnly();
                cacheUsers = _context.Users.ToList().AsReadOnly();
                cacheWorkServers = _context.WorkServers.ToList().AsReadOnly();
            }
        }

        #endregion
    }
}

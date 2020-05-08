using System.Collections.Generic;
using System.IO;
using System.Linq;
using YY.EventLogExportAssistant.PostgreSQL.Models;
using YY.EventLogReaderAssistant;
using RowData = YY.EventLogReaderAssistant.Models.RowData;
using Microsoft.EntityFrameworkCore;

namespace YY.EventLogExportAssistant.PostgreSQL
{
    public class EventLogOnPostgreSQL : EventLogOnTarget
    {
        private const int _defaultPortion = 1000;
        private int _portion;
        private EventLogContext _context;
        private InformationSystemsBase _system;

        public EventLogOnPostgreSQL() : this(null, _defaultPortion)
        {

        }
        public EventLogOnPostgreSQL(int portion) : this(null, portion)
        {
            _portion = portion;
        }
        public EventLogOnPostgreSQL(EventLogContext context, int portion)
        {
            _portion = portion;
            if (context == null)
                _context = new EventLogContext();
            else
                _context = context;
        }

        public override EventLogPosition GetLastPosition()
        {
            var lastLogFile = _context.LogFiles
                .Where(e => e.InformationSystemId == _system.Id && e.Id == _context.LogFiles.Max(m => m.Id))
                .SingleOrDefault();

            if (lastLogFile == null)
                return null;
            else
                return new EventLogPosition(
                    lastLogFile.LastEventNumber,
                    lastLogFile.LastCurrentFileReferences,
                    lastLogFile.LastCurrentFileData,
                    lastLogFile.LastStreamPosition);
        }

        public override void SaveLogPosition(FileInfo logFileInfo, EventLogPosition position)
        {
            LogFiles foundLogFile = _context.LogFiles
                .Where(l => l.InformationSystemId == _system.Id && l.FileName == logFileInfo.Name && l.CreateDate == logFileInfo.CreationTimeUtc)
                .FirstOrDefault();

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
                _context.Entry<LogFiles>(foundLogFile).State = EntityState.Modified;
            }

            _context.SaveChanges();
        }

        public override int GetPortionSize()
        {
            return _portion;
        }

        public override void Save(RowData rowData)
        {
            IList<RowData> rowsData = new List<RowData>();
            rowsData.Add(rowData);
            Save(rowsData);
        }

        public override void Save(IList<RowData> rowsData)
        {
            List<Models.RowData> newEntities = new List<Models.RowData>();
            foreach (var itemRow in rowsData)
            {
                if (itemRow == null)
                    continue;

                long? rowApplicationId = null;
                Models.Applications rowApplication = null;
                if (itemRow.Application != null)
                {
                    rowApplication = cacheApplications
                        .Where(e => e.InformationSystemId == _system.Id && e.Name == itemRow.Application.Name)
                        .FirstOrDefault();
                    rowApplicationId = rowApplication.Id;
                }

                long? rowComputerId = null;
                Models.Computers rowComputer = null;
                if (itemRow.Computer != null)
                {
                    rowComputer = cacheComputers
                        .Where(e => e.InformationSystemId == _system.Id && e.Name == itemRow.Computer.Name)
                        .FirstOrDefault();
                    rowComputerId = rowComputer.Id;
                }

                long? rowEventId = null;
                Models.Events rowEvent = null;
                if (itemRow.Event != null)
                {
                    rowEvent = cacheEvents
                        .Where(e => e.InformationSystemId == _system.Id && e.Name == itemRow.Event.Name)
                        .FirstOrDefault();
                    rowEventId = rowEvent.Id;
                }

                long? rowMetadataId = null;
                Models.Metadata rowMetadata = null;
                if (itemRow.Metadata != null)
                {
                    rowMetadata = cacheMetadata
                        .Where(e => e.InformationSystemId == _system.Id && e.Name == itemRow.Metadata.Name && e.Uuid == itemRow.Metadata.Uuid)
                        .FirstOrDefault();
                    rowMetadataId = rowMetadata.Id;
                }

                long? rowPrimaryPortId = null;
                Models.PrimaryPorts rowPrimaryPort = null;
                if (itemRow.PrimaryPort != null)
                {
                    rowPrimaryPort = cachePrimaryPorts.Where(e => e.InformationSystemId == _system.Id && e.Name == itemRow.PrimaryPort.Name)
                        .FirstOrDefault();
                    rowPrimaryPortId = rowPrimaryPort.Id;
                }

                long? rowSecondaryPortId = null;
                Models.SecondaryPorts rowSecondaryPort = null;
                if (itemRow.SecondaryPort != null)
                {
                    rowSecondaryPort = cacheSecondaryPorts
                        .Where(e => e.InformationSystemId == _system.Id && e.Name == itemRow.SecondaryPort.Name)
                        .FirstOrDefault();
                    rowSecondaryPortId = rowSecondaryPort.Id;
                }

                Models.Severities rowSeverity = cacheSeverities
                    .Where(e => e.InformationSystemId == _system.Id && e.Name == itemRow.Severity.ToString())
                    .FirstOrDefault();
                Models.TransactionStatuses rowTransactionStatus = cacheTransactionStatuses
                    .Where(e => e.InformationSystemId == _system.Id && e.Name == itemRow.TransactionStatus.ToString())
                    .FirstOrDefault();

                long? rowUserId = null;
                Models.Users rowUser = null;
                if (itemRow.User != null)
                {
                    rowUser = cacheUsers
                        .Where(e => e.InformationSystemId == _system.Id && e.Name == itemRow.User.Name)
                        .FirstOrDefault();
                    rowUserId = rowUser.Id;
                }

                long? rowWorkServerId = null;
                Models.WorkServers rowWorkServer = null;
                if (itemRow.WorkServer != null)
                {
                    rowWorkServer = cacheWorkServers
                        .Where(e => e.InformationSystemId == _system.Id && e.Name == itemRow.WorkServer.Name)
                        .FirstOrDefault();
                    rowWorkServerId = rowWorkServer.Id;
                }

                Models.RowData rowData = new Models.RowData()
                {
                    ApplicationId = rowApplicationId,
                    Comment = itemRow.Comment,
                    ComputerId = rowComputerId,
                    ConnectId = itemRow.ConnectId,
                    Data = itemRow.Data,
                    DataPresentation = itemRow.DataPresentation,
                    DataUUID = itemRow.DataUUID,
                    EventId = rowEventId,
                    Id = itemRow.RowID,
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

            // TODO
            //_context.BulkInsertOrUpdate(newEntities);
            _context.RowsData.AddRange(newEntities);
            _context.SaveChanges();
        }

        public override void SetInformationSystem(InformationSystemsBase system)
        {
            InformationSystems existSystem = _context.InformationSystems.Where(e => e.Name == system.Name).FirstOrDefault();
            if (existSystem == null)
            {
                _context.InformationSystems.Add(new InformationSystems()
                {
                    Name = system.Name,
                    Description = system.Description
                });
                _context.SaveChanges();
                existSystem = _context.InformationSystems.Where(e => e.Name == system.Name).FirstOrDefault();
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

        public override void UpdateReferences(ReferencesData data)
        {
            if (data.Applications != null)
            {
                foreach (var itemApplication in data.Applications)
                {
                    Models.Applications foundApplication = _context.Applications
                        .Where(e => e.InformationSystemId == _system.Id && e.Name == itemApplication.Name)
                        .FirstOrDefault();
                    if (foundApplication == null)
                    {
                        _context.Applications.Add(new Models.Applications()
                        {
                            InformationSystemId = _system.Id,
                            Name = itemApplication.Name
                        });
                    }
                }
            }
            if (data.Computers != null)
            {
                foreach (var itemComputer in data.Computers)
                {
                    Models.Computers foundComputer = _context.Computers
                        .Where(e => e.InformationSystemId == _system.Id && e.Name == itemComputer.Name)
                        .FirstOrDefault();
                    if (foundComputer == null)
                    {
                        _context.Computers.Add(new Models.Computers()
                        {
                            InformationSystemId = _system.Id,
                            Name = itemComputer.Name
                        });
                    }
                }
            }
            if (data.Events != null)
            {
                foreach (var itemEvent in data.Events)
                {
                    Models.Events foundEvents = _context.Events
                        .Where(e => e.InformationSystemId == _system.Id && e.Name == itemEvent.Name)
                        .FirstOrDefault();
                    if (foundEvents == null)
                    {
                        _context.Events.Add(new Models.Events()
                        {
                            InformationSystemId = _system.Id,
                            Name = itemEvent.Name
                        });
                    }
                }
            }
            if (data.Metadata != null)
            {
                foreach (var itemMetadata in data.Metadata)
                {
                    Models.Metadata foundMetadata = _context.Metadata
                        .Where(e => e.InformationSystemId == _system.Id
                            && e.Name == itemMetadata.Name
                            && e.Uuid == itemMetadata.Uuid)
                        .FirstOrDefault();
                    if (foundMetadata == null)
                    {
                        _context.Metadata.Add(new Models.Metadata()
                        {
                            InformationSystemId = _system.Id,
                            Name = itemMetadata.Name,
                            Uuid = itemMetadata.Uuid
                        });
                    }
                }
            }
            if (data.PrimaryPorts != null)
            {
                foreach (var itemPrimaryPort in data.PrimaryPorts)
                {
                    Models.PrimaryPorts foundPrimaryPort = _context.PrimaryPorts
                        .Where(e => e.InformationSystemId == _system.Id && e.Name == itemPrimaryPort.Name)
                        .FirstOrDefault();
                    if (foundPrimaryPort == null)
                    {
                        _context.PrimaryPorts.Add(new Models.PrimaryPorts()
                        {
                            InformationSystemId = _system.Id,
                            Name = itemPrimaryPort.Name
                        });
                    }
                }
            }
            if (data.SecondaryPorts != null)
            {
                foreach (var itemSecondaryPort in data.SecondaryPorts)
                {
                    Models.SecondaryPorts foundSecondaryPort = _context.SecondaryPorts
                        .Where(e => e.InformationSystemId == _system.Id && e.Name == itemSecondaryPort.Name)
                        .FirstOrDefault();
                    if (foundSecondaryPort == null)
                    {
                        _context.SecondaryPorts.Add(new Models.SecondaryPorts()
                        {
                            InformationSystemId = _system.Id,
                            Name = itemSecondaryPort.Name
                        });
                    }
                }
            }
            if (data.Severities != null)
            {
                foreach (var itemSeverity in data.Severities)
                {
                    Models.Severities foundSeverity = _context.Severities
                        .Where(e => e.InformationSystemId == _system.Id && e.Name == itemSeverity.ToString())
                        .FirstOrDefault();
                    if (foundSeverity == null)
                    {
                        _context.Severities.Add(new Models.Severities()
                        {
                            InformationSystemId = _system.Id,
                            Name = itemSeverity.ToString()
                        });
                    }
                }
            }
            if (data.TransactionStatuses != null)
            {
                foreach (var itemTransactionStatus in data.TransactionStatuses)
                {
                    Models.TransactionStatuses foundTransactionStatus = _context.TransactionStatuses
                        .Where(e => e.InformationSystemId == _system.Id && e.Name == itemTransactionStatus.ToString())
                        .FirstOrDefault();
                    if (foundTransactionStatus == null)
                    {
                        _context.TransactionStatuses.Add(new Models.TransactionStatuses()
                        {
                            InformationSystemId = _system.Id,
                            Name = itemTransactionStatus.ToString()
                        });
                    }
                }
            }
            if (data.Users != null)
            {
                foreach (var itemUser in data.Users)
                {
                    Models.Users foundUsers = _context.Users
                        .Where(e => e.InformationSystemId == _system.Id
                                && e.Name == itemUser.Name
                                && e.Uuid == itemUser.Uuid)
                        .FirstOrDefault();
                    if (foundUsers == null)
                    {
                        _context.Users.Add(new Models.Users()
                        {
                            InformationSystemId = _system.Id,
                            Name = itemUser.Name,
                            Uuid = itemUser.Uuid
                        });
                    }
                }
            }
            if (data.WorkServers != null)
            {
                foreach (var itemWorkServer in data.WorkServers)
                {
                    Models.WorkServers foundWorkServer = _context.WorkServers
                        .Where(e => e.InformationSystemId == _system.Id
                                && e.Name == itemWorkServer.Name)
                        .FirstOrDefault();
                    if (foundWorkServer == null)
                    {
                        _context.WorkServers.Add(new Models.WorkServers()
                        {
                            InformationSystemId = _system.Id,
                            Name = itemWorkServer.Name
                        });
                    }
                }
            }

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

        private IReadOnlyList<Models.Applications> cacheApplications;
        private IReadOnlyList<Models.Computers> cacheComputers;
        private IReadOnlyList<Models.Events> cacheEvents;
        private IReadOnlyList<Models.Metadata> cacheMetadata;
        private IReadOnlyList<Models.PrimaryPorts> cachePrimaryPorts;
        private IReadOnlyList<Models.SecondaryPorts> cacheSecondaryPorts;
        private IReadOnlyList<Models.Severities> cacheSeverities;
        private IReadOnlyList<Models.TransactionStatuses> cacheTransactionStatuses;
        private IReadOnlyList<Models.Users> cacheUsers;
        private IReadOnlyList<Models.WorkServers> cacheWorkServers;

        public override void Dispose()
        {
            _context.Dispose();
        }
    }
}

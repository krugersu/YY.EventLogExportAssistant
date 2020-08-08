using System;
using System.Collections.Generic;
using System.Linq;

namespace YY.EventLogExportAssistant.Database
{
    public sealed class ReferencesDataCache
    {
        #region Private Members

        private readonly InformationSystemsBase _system;

        #endregion

        #region Public Members

        public InformationSystemsBase System => _system;
        public IReadOnlyList<Models.Applications> Applications;
        public IReadOnlyList<Models.Computers> Computers;
        public IReadOnlyList<Models.Events> Events;
        public IReadOnlyList<Models.Metadata> Metadata;
        public IReadOnlyList<Models.PrimaryPorts> PrimaryPorts;
        public IReadOnlyList<Models.SecondaryPorts> SecondaryPorts;
        public IReadOnlyList<Models.Severities> Severities;
        public IReadOnlyList<Models.TransactionStatuses> TransactionStatuses;
        public IReadOnlyList<Models.Users> Users;
        public IReadOnlyList<Models.WorkServers> WorkServers;

        #endregion

        #region Constructors

        public ReferencesDataCache(InformationSystemsBase system)
        {
            _system = system;
        }

        #endregion

        #region Public Methods

        public long? GetReferenceDatabaseId<T>(YY.EventLogReaderAssistant.Models.RowData itemRow)
        {
            long? id;

            if (typeof(T) == typeof(YY.EventLogReaderAssistant.Models.Applications))
                id = GetApplicationId(itemRow.Application);
            else if (typeof(T) == typeof(YY.EventLogReaderAssistant.Models.Computers))
                id = GetComputerId(itemRow.Computer);
            else if (typeof(T) == typeof(YY.EventLogReaderAssistant.Models.Events))
                id = GetEventId(itemRow.Event);
            else if (typeof(T) == typeof(YY.EventLogReaderAssistant.Models.Metadata))
                id = GetMetadataId(itemRow.Metadata);
            else if (typeof(T) == typeof(YY.EventLogReaderAssistant.Models.PrimaryPorts))
                id = GetPrimaryPortId(itemRow.PrimaryPort);
            else if (typeof(T) == typeof(YY.EventLogReaderAssistant.Models.SecondaryPorts))
                id = GetSecondaryPortId(itemRow.SecondaryPort);
            else if (typeof(T) == typeof(YY.EventLogReaderAssistant.Models.Severity))
                id = GetSeverityId(itemRow.Severity);
            else if (typeof(T) == typeof(YY.EventLogReaderAssistant.Models.TransactionStatus))
                id = GetTransactionStatusId(itemRow.TransactionStatus);
            else if (typeof(T) == typeof(YY.EventLogReaderAssistant.Models.Users))
                id = GetUserId(itemRow.User);
            else if (typeof(T) == typeof(YY.EventLogReaderAssistant.Models.WorkServers))
                id = GetWorkServerId(itemRow.WorkServer);
            else throw new Exception("Неизвестный тип ссылочного объекта");

            return id;
        }
        public void FillByDatabaseContext(EventLogContext context)
        {
            Applications = context.Applications.ToList().AsReadOnly();
            Computers = context.Computers.ToList().AsReadOnly();
            Events = context.Events.ToList().AsReadOnly();
            Metadata = context.Metadata.ToList().AsReadOnly();
            PrimaryPorts = context.PrimaryPorts.ToList().AsReadOnly();
            SecondaryPorts = context.SecondaryPorts.ToList().AsReadOnly();
            Severities = context.Severities.ToList().AsReadOnly();
            TransactionStatuses = context.TransactionStatuses.ToList().AsReadOnly();
            Users = context.Users.ToList().AsReadOnly();
            WorkServers = context.WorkServers.ToList().AsReadOnly();
        }

        #endregion

        #region Private Methods

        private long? GetApplicationId(YY.EventLogReaderAssistant.Models.Applications item)
        {
            long? id;

            if (item == null)
                id = null;
            else
                id = Applications.First(e => e.InformationSystemId == _system.Id && e.Name == item.Name).Id;

            return id;
        }
        private long? GetComputerId(YY.EventLogReaderAssistant.Models.Computers item)
        {
            long? id;

            if (item == null)
                id = null;
            else
                id = Computers.First(e => e.InformationSystemId == _system.Id && e.Name == item.Name).Id;

            return id;
        }
        private long? GetEventId(YY.EventLogReaderAssistant.Models.Events item)
        {
            long? id;

            if (item == null)
                id = null;
            else
                id = Events.First(e => e.InformationSystemId == _system.Id && e.Name == item.Name).Id;

            return id;
        }
        private long? GetMetadataId(YY.EventLogReaderAssistant.Models.Metadata item)
        {
            long? id;

            if (item == null)
                id = null;
            else
                id = Metadata.First(e => e.InformationSystemId == _system.Id && e.Name == item.Name && e.Uuid == item.Uuid).Id;

            return id;
        }
        private long? GetPrimaryPortId(YY.EventLogReaderAssistant.Models.PrimaryPorts item)
        {
            long? id;

            if (item == null)
                id = null;
            else
                id = PrimaryPorts.First(e => e.InformationSystemId == _system.Id && e.Name == item.Name).Id;

            return id;
        }
        private long? GetSecondaryPortId(YY.EventLogReaderAssistant.Models.SecondaryPorts item)
        {
            long? id;

            if (item == null)
                id = null;
            else
                id = SecondaryPorts.First(e => e.InformationSystemId == _system.Id && e.Name == item.Name).Id;

            return id;
        }
        private long? GetSeverityId(YY.EventLogReaderAssistant.Models.Severity item)
        {
            long? id = Severities.First(e => e.InformationSystemId == _system.Id && e.Name == item.ToString()).Id;
            return id;
        }
        private long? GetTransactionStatusId(YY.EventLogReaderAssistant.Models.TransactionStatus item)
        {
            long? id = TransactionStatuses.First(e => e.InformationSystemId == _system.Id && e.Name == item.ToString()).Id;
            return id;
        }
        private long? GetUserId(YY.EventLogReaderAssistant.Models.Users item)
        {
            long? id;

            if (item == null)
                id = null;
            else
                id = Users.First(e => e.InformationSystemId == _system.Id && e.Name == item.Name && item.Uuid == e.Uuid).Id;

            return id;
        }
        private long? GetWorkServerId(YY.EventLogReaderAssistant.Models.WorkServers item)
        {
            long? id;

            if (item == null)
                id = null;
            else
                id = WorkServers.First(e => e.InformationSystemId == _system.Id && e.Name == item.Name).Id;

            return id;
        }

        #endregion
    }
}

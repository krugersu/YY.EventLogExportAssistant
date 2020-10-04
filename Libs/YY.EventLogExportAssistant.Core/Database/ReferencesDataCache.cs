using System;
using System.Collections.Generic;
using System.Linq;
using YY.EventLogExportAssistant.Helpers;

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

        public IDictionary<string, List<Models.Applications>> ApplicationsDictionary;
        public IDictionary<string, List<Models.Computers>> ComputersDictionary;
        public IDictionary<string, List<Models.Events>> EventsDictionary;
        public IDictionary<string, List<Models.Metadata>> MetadataDictionary;
        public IDictionary<string, List<Models.PrimaryPorts>> PrimaryPortsDictionary;
        public IDictionary<string, List<Models.SecondaryPorts>> SecondaryPortsDictionary;
        public IDictionary<string, List<Models.Severities>> SeveritiesDictionary;
        public IDictionary<string, List<Models.TransactionStatuses>> TransactionStatusesDictionary;
        public IDictionary<string, List<Models.Users>> UsersDictionary;
        public IDictionary<string, List<Models.WorkServers>> WorkServersDictionary;

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
            Applications = context.Applications.Where(e => e.InformationSystemId == _system.Id).ToList().AsReadOnly();
            Computers = context.Computers.Where(e => e.InformationSystemId == _system.Id).ToList().AsReadOnly();
            Events = context.Events.Where(e => e.InformationSystemId == _system.Id).ToList().AsReadOnly();
            Metadata = context.Metadata.Where(e => e.InformationSystemId == _system.Id).ToList().AsReadOnly();
            PrimaryPorts = context.PrimaryPorts.Where(e => e.InformationSystemId == _system.Id).ToList().AsReadOnly();
            SecondaryPorts = context.SecondaryPorts.Where(e => e.InformationSystemId == _system.Id).ToList().AsReadOnly();
            Severities = context.Severities.Where(e => e.InformationSystemId == _system.Id).ToList().AsReadOnly();
            TransactionStatuses = context.TransactionStatuses.Where(e => e.InformationSystemId == _system.Id).ToList().AsReadOnly();
            Users = context.Users.Where(e => e.InformationSystemId == _system.Id).ToList().AsReadOnly();
            WorkServers = context.WorkServers.Where(e => e.InformationSystemId == _system.Id).ToList().AsReadOnly();

            ApplicationsDictionary = Applications.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            ComputersDictionary = Computers.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            EventsDictionary = Events.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            MetadataDictionary = Metadata.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            PrimaryPortsDictionary = PrimaryPorts.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            SecondaryPortsDictionary = SecondaryPorts.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            SeveritiesDictionary = Severities.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            TransactionStatusesDictionary = TransactionStatuses.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            UsersDictionary = Users.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            WorkServersDictionary = WorkServers.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
        }
        
        #endregion

        #region Private Methods

        private long? GetApplicationId(EventLogReaderAssistant.Models.Applications item)
        {
            if (item == null) return null;

            return ApplicationsDictionary[item.Name.Truncate(500)].First().Id;
        }
        private long? GetComputerId(EventLogReaderAssistant.Models.Computers item)
        {
            if (item == null) return null;

            return ComputersDictionary[item.Name.Truncate(500)].First().Id;
        }
        private long? GetEventId(EventLogReaderAssistant.Models.Events item)
        {
            if (item == null) return null;

            return EventsDictionary[item.Name.Truncate(500)].First().Id;
        }
        private long? GetMetadataId(EventLogReaderAssistant.Models.Metadata item)
        {
            if (item == null) return null;

            return MetadataDictionary[item.Name.Truncate(500)].First(e => e.Uuid == item.Uuid).Id;
        }
        private long? GetPrimaryPortId(EventLogReaderAssistant.Models.PrimaryPorts item)
        {
            if (item == null) return null;

            return PrimaryPortsDictionary[item.Name.Truncate(500)].First().Id;
        }
        private long? GetSecondaryPortId(EventLogReaderAssistant.Models.SecondaryPorts item)
        {
            if (item == null) return null;

            return SecondaryPortsDictionary[item.Name.Truncate(500)].First().Id;
        }
        private long? GetSeverityId(EventLogReaderAssistant.Models.Severity item)
        {
            return SeveritiesDictionary[item.ToString()].First().Id;
        }
        private long? GetTransactionStatusId(EventLogReaderAssistant.Models.TransactionStatus item)
        {
            return TransactionStatusesDictionary[item.ToString()].First().Id;
        }
        private long? GetUserId(EventLogReaderAssistant.Models.Users item)
        {
            if (item == null) return null;

            return UsersDictionary[item.Name.Truncate(500)].First(e => e.Uuid == item.Uuid).Id;
        }
        private long? GetWorkServerId(EventLogReaderAssistant.Models.WorkServers item)
        {
            if (item == null) return null;

            return WorkServersDictionary[item.Name.Truncate(500)].First().Id;
        }

        #endregion
    }
}

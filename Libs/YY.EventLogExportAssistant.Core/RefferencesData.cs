using System;
using System.Collections.Generic;
using System.Linq;
using YY.EventLogReaderAssistant;
using YY.EventLogReaderAssistant.Models;
using ReferenceObject = YY.EventLogExportAssistant.Database.Models.ReferenceObject;

namespace YY.EventLogExportAssistant
{
    public sealed class ReferencesData
    {
        #region Public Members

        public readonly IReadOnlyList<Applications> Applications;
        public readonly IReadOnlyList<Computers> Computers;
        public readonly IReadOnlyList<Events> Events;
        public readonly IReadOnlyList<Metadata> Metadata;
        public readonly IReadOnlyList<PrimaryPorts> PrimaryPorts;
        public readonly IReadOnlyList<SecondaryPorts> SecondaryPorts;
        public readonly IReadOnlyList<Severity> Severities;
        public readonly IReadOnlyList<TransactionStatus> TransactionStatuses;
        public readonly IReadOnlyList<Users> Users;
        public readonly IReadOnlyList<WorkServers> WorkServers;

        #endregion

        #region Constructors
        
        private ReferencesData()
        {
            Severities = new List<Severity>
            {
                Severity.Error,
                Severity.Information,
                Severity.Note,
                Severity.Unknown,
                Severity.Warning
            }.AsReadOnly();

            TransactionStatuses = new List<TransactionStatus>
            {
                TransactionStatus.Committed,
                TransactionStatus.NotApplicable,
                TransactionStatus.RolledBack,
                TransactionStatus.Unfinished,
                TransactionStatus.Unknown
            }.AsReadOnly();
        }
        public ReferencesData(EventLogReader reader) : this()
        {
            Applications = reader.Applications.ToList().AsReadOnly();
            Computers = reader.Computers.ToList().AsReadOnly();
            Events = reader.Events.ToList().AsReadOnly();
            Metadata = reader.Metadata.ToList().AsReadOnly();
            PrimaryPorts = reader.PrimaryPorts.ToList().AsReadOnly();
            SecondaryPorts = reader.SecondaryPorts.ToList().AsReadOnly();
            Users = reader.Users.ToList().AsReadOnly();
            WorkServers = reader.WorkServers.ToList().AsReadOnly();
        }

        #endregion

        #region Public Methods

        public IReadOnlyList<T> GetReferencesListForDatabaseType<T>(InformationSystemsBase system) where T : ReferenceObject
        {
            IReadOnlyList<T> result;
            if (typeof(T) == typeof(Database.Models.Applications))
                result = (IReadOnlyList<T>)Applications.Select(i => new Database.Models.Applications() { Name = i.Name, InformationSystemId = system.Id }).ToList().AsReadOnly();
            else if (typeof(T) == typeof(Database.Models.Computers))
                result = (IReadOnlyList<T>)Computers.Select(i => new Database.Models.Computers() { Name = i.Name, InformationSystemId = system.Id }).ToList().AsReadOnly();
            else if (typeof(T) == typeof(Database.Models.Events))
                result = (IReadOnlyList<T>)Events.Select(i => new Database.Models.Events() { Name = i.Name, InformationSystemId = system.Id }).ToList().AsReadOnly();
            else if (typeof(T) == typeof(Database.Models.Metadata))
                result = (IReadOnlyList<T>)Metadata.Select(i => new Database.Models.Metadata() { Name = i.Name, InformationSystemId = system.Id, Uuid = i.Uuid }).ToList().AsReadOnly();
            else if (typeof(T) == typeof(Database.Models.PrimaryPorts))
                result = (IReadOnlyList<T>)PrimaryPorts.Select(i => new Database.Models.PrimaryPorts() { Name = i.Name, InformationSystemId = system.Id }).ToList().AsReadOnly();
            else if (typeof(T) == typeof(Database.Models.SecondaryPorts))
                result = (IReadOnlyList<T>)SecondaryPorts.Select(i => new Database.Models.SecondaryPorts() { Name = i.Name, InformationSystemId = system.Id }).ToList().AsReadOnly();
            else if (typeof(T) == typeof(Database.Models.Severities))
                result = (IReadOnlyList<T>)Severities.Select(i => new Database.Models.Severities() { Name = i.ToString(), InformationSystemId = system.Id }).ToList().AsReadOnly();
            else if (typeof(T) == typeof(Database.Models.TransactionStatuses))
                result = (IReadOnlyList<T>)TransactionStatuses.Select(i => new Database.Models.TransactionStatuses() { Name = i.ToString(), InformationSystemId = system.Id }).ToList().AsReadOnly();
            else if (typeof(T) == typeof(Database.Models.Users))
                result = (IReadOnlyList<T>)Users.Select(i => new Database.Models.Users() { Name = i.Name, InformationSystemId = system.Id, Uuid = i.Uuid }).ToList().AsReadOnly();
            else if (typeof(T) == typeof(Database.Models.WorkServers))
                result = (IReadOnlyList<T>) WorkServers.Select(i => new Database.Models.WorkServers() { Name = i.Name, InformationSystemId = system.Id }).ToList().AsReadOnly();
            else throw new Exception("Неизвестный тип для формирования списка исходных ссылок");
            return result;
        }

        #endregion
    }
}

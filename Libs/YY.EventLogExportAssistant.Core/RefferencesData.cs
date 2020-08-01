using System;
using System.Collections.Generic;
using System.Linq;
using YY.EventLogExportAssistant.Database;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportAssistant
{
    public sealed class ReferencesData
    {
        #region Public Members

        public IReadOnlyList<Applications> Applications;
        public IReadOnlyList<Computers> Computers;
        public IReadOnlyList<Events> Events;
        public IReadOnlyList<Metadata> Metadata;
        public IReadOnlyList<PrimaryPorts> PrimaryPorts;
        public IReadOnlyList<SecondaryPorts> SecondaryPorts;
        public IReadOnlyList<Severity> Severities;
        public IReadOnlyList<TransactionStatus> TransactionStatuses;
        public IReadOnlyList<Users> Users;
        public IReadOnlyList<WorkServers> WorkServers;

        #endregion

        #region Public Methods

        public IReadOnlyList<T> GetReferencesListForDatabaseType<T>(InformationSystemsBase system) where T : IDatabaseReferenceItem
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
            else
                throw new Exception("Неизвестный тип для формирования списка исходных ссылок");
            return result;
        }

        #endregion
    }
}

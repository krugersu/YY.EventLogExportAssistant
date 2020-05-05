using System;
using System.Collections.Generic;
using System.Text;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportAssistant
{
    public class ReferencesData
    {
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
    }
}

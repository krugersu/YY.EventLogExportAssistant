using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YY.EventLogExportAssistant.PostgreSQL.Models
{
    public class RowData : LogObject
    {
        public DateTimeOffset Period { get; set; }
        public long Id { get; set; }
        public Severities Severity { get; set; }
        public long? ConnectId { get; set; }
        public long? Session { get; set; }
        public TransactionStatuses TransactionStatus { get; set; }
        public DateTime? TransactionDate { get; set; }
        public long? TransactionId { get; set; }
        public Users User { get; set; }
        public Computers Computer { get; set; }
        public Applications Application { get; set; }
        public Events Event { get; set; }
        public string Comment { get; set; }
        public Metadata Metadata { get; set; }
        public string Data { get; set; }
        public string DataUUID { get; set; }
        public string DataPresentation { get; set; }
        public WorkServers WorkServer { get; set; }
        public PrimaryPorts PrimaryPort { get; set; }
        public SecondaryPorts SecondaryPort { get; set; }
    }
}

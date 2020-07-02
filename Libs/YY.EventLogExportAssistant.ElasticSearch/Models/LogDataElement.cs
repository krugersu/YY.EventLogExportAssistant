using System;
using System.Collections.Generic;
using System.Text;

namespace YY.EventLogExportAssistant.ElasticSearch.Models
{
    public class LogDataElement
    {
        public string Id { get; set; }
        public string InformationSystem { get; set; }
        public DateTimeOffset Period { get; set; }
        public string Severity { get; set; }
        public long? ConnectionId { get; set; }
        public long? Session { get; set; }
        public string TransactionStatus { get; set; }
        public DateTime? TransactionDate { get; set; }
        public long? TransactionId { get; set; }
        public string User { get; set; }
        public string UserUUID { get; set; }
        public string Computer { get; set; }
        public string Application { get; set; }
        public string Event { get; set; }
        public string Comment { get; set; }
        public string Metadata { get; set; }
        public string MetadataUUID { get; set; }
        public string Data { get; set; }
        public string DataUUID { get; set; }
        public string DataPresentation { get; set; }
        public string WorkServer { get; set; }
        public string PrimaryPort { get; set; }
        public string SecondaryPort { get; set; }
    }
}

using System;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class RowData : CommonLogObject
    {
        public DateTimeOffset Period { get; set; }
        public long? SeverityId { get; set; }
        public long? ConnectId { get; set; }
        public long? Session { get; set; }
        public long? TransactionStatusId { get; set; }        
        public DateTime? TransactionDate { get; set; }
        public long? TransactionId { get; set; }
        public long? UserId { get; set; }        
        public long? ComputerId { get; set; }        
        public long? ApplicationId { get; set; }        
        public long? EventId { get; set; }        
        public string Comment { get; set; }
        public long? MetadataId { get; set; }        
        public string Data { get; set; }
        public string DataUUID { get; set; }
        public string DataPresentation { get; set; }
        public long? WorkServerId { get; set; }        
        public long? PrimaryPortId { get; set; }        
        public long? SecondaryPortId { get; set; }
    }
}

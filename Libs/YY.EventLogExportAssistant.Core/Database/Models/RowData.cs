using System;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class RowData : CommonLogObject
    {
        #region Public Members

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

        #endregion

        #region Constructors

        public RowData()
        {
        }
        public RowData(InformationSystemsBase system, EventLogReaderAssistant.Models.RowData sourceRow, ReferencesDataCache referencesCache)
        {
            ApplicationId = referencesCache.GetReferenceDatabaseId<EventLogReaderAssistant.Models.Applications>(sourceRow);
            Comment = sourceRow.Comment;
            ComputerId = referencesCache.GetReferenceDatabaseId<EventLogReaderAssistant.Models.Computers>(sourceRow);
            ConnectId = sourceRow.ConnectId;
            Data = sourceRow.Data;
            DataPresentation = sourceRow.DataPresentation;
            DataUUID = sourceRow.DataUuid;
            EventId = referencesCache.GetReferenceDatabaseId<EventLogReaderAssistant.Models.Events>(sourceRow);
            Id = sourceRow.RowId;
            InformationSystemId = system.Id;
            MetadataId = referencesCache.GetReferenceDatabaseId<EventLogReaderAssistant.Models.Metadata>(sourceRow);
            Period = sourceRow.Period;
            PrimaryPortId = referencesCache.GetReferenceDatabaseId<EventLogReaderAssistant.Models.PrimaryPorts>(sourceRow);
            SecondaryPortId = referencesCache.GetReferenceDatabaseId<EventLogReaderAssistant.Models.SecondaryPorts>(sourceRow);
            Session = sourceRow.Session;
            SeverityId = referencesCache.GetReferenceDatabaseId<EventLogReaderAssistant.Models.Severity>(sourceRow);
            TransactionDate = sourceRow.TransactionDate;
            TransactionId = sourceRow.TransactionId;
            TransactionStatusId = referencesCache.GetReferenceDatabaseId<EventLogReaderAssistant.Models.TransactionStatus>(sourceRow);
            UserId = referencesCache.GetReferenceDatabaseId<EventLogReaderAssistant.Models.Users>(sourceRow);
            WorkServerId = referencesCache.GetReferenceDatabaseId<EventLogReaderAssistant.Models.WorkServers>(sourceRow);
        }

        #endregion
    }
}

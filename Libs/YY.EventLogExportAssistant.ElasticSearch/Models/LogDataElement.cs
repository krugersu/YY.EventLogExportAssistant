using System;
using YY.EventLogExportAssistant.Database.Models;
using RowData = YY.EventLogReaderAssistant.Models.RowData;

namespace YY.EventLogExportAssistant.ElasticSearch.Models
{
    public class LogDataElement
    {
        #region Public Properties

        public string Id { get; set; }
        public string InformationSystem { get; set; }
        public DateTimeOffset Period { get; set; }
        public long RowId { get; set; }
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

        #endregion

        #region Constructors

        public LogDataElement(InformationSystemsBase system, RowData item)
        {
            Id = $"[{system.Name}][{item.Period:yyyyMMddhhmmss}][{item.RowId}]";
            Application = item.Application.Name;
            Comment = item.Comment;
            Computer = item.Computer?.Name;
            ConnectionId = item.ConnectId;
            Data = item.Data;
            DataPresentation = item.DataPresentation;
            DataUUID = item.DataUuid;
            Event = item.Event?.Name;
            RowId = item.RowId;
            InformationSystem = system.Name;
            Metadata = item.Metadata?.Name;
            MetadataUUID = item.Metadata?.Uuid.ToString();
            Period = item.Period;
            PrimaryPort = item.PrimaryPort?.Name;
            SecondaryPort = item.SecondaryPort?.Name;
            Session = item.Session;
            Severity = item.Severity.ToString();
            TransactionDate = item.TransactionDate;
            TransactionId = item.TransactionId;
            TransactionStatus = item.TransactionStatus.ToString();
            User = item.User?.Name;
            UserUUID = item.User?.Uuid.ToString();
            WorkServer = item.WorkServer?.Name;
        }

        #endregion
    }
}

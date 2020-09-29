using System;
using System.Collections;
using YY.EventLogExportAssistant.Database;
using YY.EventLogExportAssistant.Database.Models;

namespace YY.EventLogExportAssistant.ClickHouse.Models
{
    public class RowDataBulkInsert : RowData, IEnumerable
    {
        #region Public Members

        public DateTime PeriodAsDateTime => Period.DateTime;
        public readonly string InformationSystemAsString;
        public readonly string SeverityAsString;
        public readonly string TransactionStatusAsString;
        public readonly string UserAsString;
        public readonly string ComputerAsString;
        public readonly string ApplicationAsString;
        public readonly string EventAsString;
        public readonly string MetadataAsString;
        public readonly string WorkServerAsString;
        public readonly string PrimaryPortAsString;
        public readonly string SecondaryPortAsString;

        #endregion

        #region Public Methods

        public IEnumerator GetEnumerator()
        {
            yield return InformationSystemAsString;
            yield return Id;
            yield return PeriodAsDateTime;
            yield return SeverityAsString;
            yield return ConnectId;
            yield return Session;
            yield return TransactionStatusAsString;
            yield return TransactionDate;
            yield return TransactionId;
            yield return UserAsString;
            yield return ComputerAsString;
            yield return ApplicationAsString;
            yield return EventAsString;
            yield return Comment;
            yield return MetadataAsString;
            yield return Data; 
            yield return DataUUID;
            yield return DataPresentation;
            yield return WorkServerAsString;
            yield return PrimaryPortAsString;
            yield return SecondaryPortAsString;
        }

        #endregion

        #region Constructors

        public RowDataBulkInsert(InformationSystemsBase system, EventLogReaderAssistant.Models.RowData sourceRow, ReferencesDataCache referencesCache)
            :base(system, sourceRow, referencesCache)
        {
            if (SeverityId == null) SeverityId = -1;
            if (ConnectId == null) ConnectId = -1;
            if (Session == null) Session = -1;
            if (TransactionStatusId == null) TransactionStatusId = -1;
            if (TransactionDate == null) TransactionDate = DateTime.MinValue;
            if (TransactionId == null) TransactionId = -1;
            if (UserId == null) UserId = -1;
            if (ComputerId == null) ComputerId = -1;
            if (ApplicationId == null) ApplicationId = -1;
            if (EventId == null) EventId = -1;
            if (Comment == null) Comment = string.Empty;
            if (MetadataId == null) MetadataId = -1;
            if (Data == null) Data = string.Empty;
            if (DataUUID == null) DataUUID = string.Empty;
            if (DataPresentation == null) DataPresentation = string.Empty;
            if (WorkServerId == null) WorkServerId = -1;
            if (PrimaryPortId == null) PrimaryPortId = -1;
            if (SecondaryPortId == null) SecondaryPortId = -1;

            InformationSystemAsString = system.Name;
            SeverityAsString = Severities.GetPresentationByName(sourceRow.Severity.ToString());
            TransactionStatusAsString = TransactionStatuses.GetPresentationByName(sourceRow.TransactionStatus.ToString());
            UserAsString = sourceRow.User != null ? sourceRow.User.Name : string.Empty;
            ComputerAsString = sourceRow.Computer != null ? sourceRow.Computer.Name : string.Empty;
            ApplicationAsString = sourceRow.Application != null ? Applications.GetPresentationByName(sourceRow.Application.Name) : string.Empty;
            EventAsString = sourceRow.Event != null ? Events.GetPresentationByName(sourceRow.Event.Name) : string.Empty;
            MetadataAsString = sourceRow.Metadata != null ? sourceRow.Metadata.Name : string.Empty;
            WorkServerAsString = sourceRow.WorkServer != null ? sourceRow.WorkServer.Name : string.Empty;
            PrimaryPortAsString = sourceRow.PrimaryPort != null ? sourceRow.PrimaryPort.Name : string.Empty;
            SecondaryPortAsString = sourceRow.SecondaryPort != null ? sourceRow.SecondaryPort.Name : string.Empty;
        }

        #endregion
    }
}

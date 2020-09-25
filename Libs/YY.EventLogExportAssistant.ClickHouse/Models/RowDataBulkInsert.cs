using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using YY.EventLogExportAssistant.Database;
using YY.EventLogExportAssistant.Database.Models;

namespace YY.EventLogExportAssistant.ClickHouse.Models
{
    public class RowDataBulkInsert : RowData, IEnumerable
    {
        #region Public Members

        public DateTime PeriodAsDateTime
        {
            get
            {
                return Period.DateTime;
            }
        }

        #endregion

        #region Public Methods

        public IEnumerator GetEnumerator()
        {
            yield return InformationSystemId;
            yield return Id;
            yield return PeriodAsDateTime;
            yield return SeverityId;
            yield return ConnectId;
            yield return Session;
            yield return TransactionStatusId;
            yield return TransactionDate;
            yield return TransactionId;
            yield return UserId;
            yield return ComputerId;
            yield return ApplicationId;
            yield return EventId;
            yield return Comment;
            yield return MetadataId;
            yield return Data; 
            yield return DataUUID;
            yield return DataPresentation;
            yield return WorkServerId;
            yield return PrimaryPortId;
            yield return SecondaryPortId;
        }

        #endregion

        #region Constructors

        public RowDataBulkInsert()
        {
            
        }
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
        }

        #endregion
    }
}

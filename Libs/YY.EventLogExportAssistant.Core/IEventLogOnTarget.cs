using System;
using System.Collections.Generic;
using System.IO;
using YY.EventLogReaderAssistant;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportAssistant
{
    public interface IEventLogOnTarget
    {
        EventLogPosition GetLastPosition();
        void SaveLogPosition(FileInfo logFileInfo, EventLogPosition position);
        int GetPortionSize();
        void SetInformationSystem(InformationSystemsBase system);
        void Save(RowData rowData);
        void Save(IList<RowData> rowsData);
        void UpdateReferences(ReferencesData data);
        TimeZoneInfo GetTimeZone();
    }
}

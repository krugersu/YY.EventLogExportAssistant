using System.Collections.Generic;
using YY.EventLogReaderAssistant;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportAssistant
{
    public interface IEventLogOnTarget
    {
        EventLogPosition GetLastPosition();
        int GetPortionSize();
        void SetInformationSystem(InformationSystemsBase system);
        void Save(RowData rowData);
        void Save(IList<RowData> rowsData);
        void UpdateReferences(ReferencesData data);
    }
}

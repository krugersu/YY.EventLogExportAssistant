using System.Collections.Generic;
using YY.EventLogExportAssistant.ClickHouse.Models;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportAssistant.ClickHouse
{
    public interface IExtendedActions
    {
        void OnDatabaseModelConfiguring(object context, OnDatabaseModelConfiguringParameters parameters);
        void BeforeSaveData(
            InformationSystemsBase system, 
            List<RowData> rowsData,
            ref IEnumerable<object[]> dataToSave);
    }
}

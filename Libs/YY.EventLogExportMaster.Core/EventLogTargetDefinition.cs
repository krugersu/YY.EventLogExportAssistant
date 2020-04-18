using System;
using System.Collections.Generic;
using System.Text;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportMaster
{
    public abstract class EventLogTargetDefinition : IEventLogTargetDefinition
    {
        public void Save(RowData rowData)
        {
        }

        public void Save(IList<RowData> rowsData)
        {
        }
    }
}

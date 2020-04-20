using System;
using System.Collections.Generic;
using System.Text;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportAssistant
{
    public abstract class EventLogTargetDefinition : IEventLogTargetDefinition
    {
        public virtual void Save(RowData rowData)
        {
        }

        public virtual void Save(IList<RowData> rowsData)
        {
        }
    }
}

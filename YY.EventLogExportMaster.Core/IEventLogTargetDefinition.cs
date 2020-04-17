using System;
using System.Collections.Generic;
using System.Text;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportMaster
{
    public interface IEventLogTargetDefinition
    {
        void Save(RowData rowData);
        void Save(IList<RowData> rowsData);        
    }
}

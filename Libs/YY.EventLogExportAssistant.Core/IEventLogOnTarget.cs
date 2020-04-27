using System.Collections.Generic;

namespace YY.EventLogExportAssistant
{
    public interface IEventLogOnTarget
    {
        void Save(CommonLogObject rowData);
        void Save(IList<CommonLogObject> rowsData);        
    }
}

using System.Collections.Generic;

namespace YY.EventLogExportAssistant
{
    public interface IEventLogTarget<T> where T : CommonLogObject
    {
        void Save(T rowData);
        void Save(IList<T> rowsData);        
    }
}

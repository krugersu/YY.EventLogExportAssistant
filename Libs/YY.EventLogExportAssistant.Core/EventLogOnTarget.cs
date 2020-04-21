using System;
using System.Collections.Generic;

namespace YY.EventLogExportAssistant
{
    public abstract class EventLogOnTarget<T> : IEventLogTarget<T>, IDisposable where T : CommonLogObject
    {
        public virtual void Save(T rowData)
        {
            throw new NotImplementedException();
        }

        public virtual void Save(IList<T> rowsData)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

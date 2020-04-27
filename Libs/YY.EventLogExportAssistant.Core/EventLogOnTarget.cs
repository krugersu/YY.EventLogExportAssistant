using System;
using System.Collections.Generic;

namespace YY.EventLogExportAssistant
{
    public abstract class EventLogOnTarget : IEventLogOnTarget, IDisposable
    {
        public virtual void Save(CommonLogObject rowData)
        {
            throw new NotImplementedException();
        }

        public virtual void Save(IList<CommonLogObject> rowsData)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

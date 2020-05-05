using System;
using System.Collections.Generic;
using YY.EventLogReaderAssistant;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportAssistant
{
    public abstract class EventLogOnTarget : IEventLogOnTarget, IDisposable
    {
        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }

        public virtual EventLogPosition GetLastPosition()
        {
            throw new NotImplementedException();
        }

        public virtual int GetPortionSize()
        {
            throw new NotImplementedException();
        }

        public virtual void Save(RowData rowData)
        {
            throw new NotImplementedException();
        }

        public virtual void Save(IList<RowData> rowsData)
        {
            throw new NotImplementedException();
        }

        public virtual void SetInformationSystem(InformationSystems system)
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateReferences(ReferencesData data)
        {
            throw new NotImplementedException();
        }
             
    }
}

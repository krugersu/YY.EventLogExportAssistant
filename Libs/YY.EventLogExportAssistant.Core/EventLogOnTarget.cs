using System;
using System.Collections.Generic;
using System.IO;
using YY.EventLogReaderAssistant;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportAssistant
{
    public abstract class EventLogOnTarget : IEventLogOnTarget
    {
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

        public virtual void SaveLogPosition(FileInfo logFileInfo, EventLogPosition position)
        {
            throw new NotImplementedException();
        }

        public virtual void SetInformationSystem(InformationSystemsBase system)
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateReferences(ReferencesData data)
        {
            throw new NotImplementedException();
        }
             
    }
}

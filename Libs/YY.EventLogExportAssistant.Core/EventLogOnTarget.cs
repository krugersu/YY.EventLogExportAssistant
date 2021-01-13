using System;
using System.Collections.Generic;
using System.IO;
using YY.EventLogReaderAssistant;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportAssistant
{
    public abstract class EventLogOnTarget : IEventLogOnTarget
    {
        #region Private Members

        protected const int _defaultPortion = 1000;
        protected int _portion;
        protected InformationSystemsBase _system;
        protected EventLogPosition _lastEventLogFilePosition;
        protected DateTime _maxPeriodRowData;

        #endregion

        #region Public Methods

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
        public virtual void UpdateReferences(ReferencesData data)
        {
        } 
        public virtual void SetInformationSystem(InformationSystemsBase system)
        {
            _system = system;
        }
        public TimeZoneInfo GetTimeZone()
        {
            string timeZoneName = _system.TimeZoneName;
            if (string.IsNullOrEmpty(timeZoneName))
            {
                return TimeZoneInfo.Local;
            }
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
        }

        #endregion
    }
}

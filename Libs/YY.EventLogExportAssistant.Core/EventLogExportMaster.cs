using System;
using YY.EventLogReaderAssistant;

namespace YY.EventLogExportAssistant
{
    public sealed class EventLogExportMaster : IEventLogExportMaster, IDisposable
    {        
        private string _eventLogPath;
        private EventLogReader _eventLogReader;
        private IEventLogOnTarget _target;
        private int _watchPeriod;

        public delegate void BeforeExportDataHandler(BeforeExportDataEventArgs e);
        public event BeforeExportDataHandler BeforeExportData;

        public delegate void AfterExportDataHandler(AfterExportDataEventArgs e);
        public event AfterExportDataHandler AfterExportData;

        public EventLogExportMaster()
        {

        }

        public void SetEventLogPath(string eventLogPath)
        {
            _eventLogPath = eventLogPath;
        }
        public void SetWatchPeriod(int seconds)
        {
            _watchPeriod = seconds;
        }
        public void SetTarget(IEventLogOnTarget target)
        {
            _target = target;
        }
        public void BeginWatch()
        {
            throw new NotImplementedException();
        }
        public void EndWatch()
        {
            throw new NotImplementedException();
        }
        public bool NewDataAvailiable()
        {
            throw new NotImplementedException();
        }
        public void SendData()
        {
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            
        }
    }
}

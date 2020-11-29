using System;

namespace YY.EventLogExportAssistant
{
    public interface IEventLogExportMaster
    {
        void SetEventLogPath(string eventLogPath);
        void SetTarget(IEventLogOnTarget target);
        bool NewDataAvailable();
        void SendData();
        TimeZoneInfo GetTimeZone();
    }
}


namespace YY.EventLogExportAssistant
{
    public interface IEventLogExportMaster
    {
        void SetEventLogPath(string eventLogPath);
        void SetWatchPeriod(int seconds);
        void SetTarget(IEventLogOnTarget target);
        void BeginWatch();
        void EndWatch();
        bool NewDataAvailiable();
        void SendData();
    }
}

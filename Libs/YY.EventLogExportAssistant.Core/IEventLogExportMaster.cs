namespace YY.EventLogExportAssistant
{
    public interface IEventLogExportMaster
    {
        void SetEventLogPath(string eventLogPath);
        void SetTarget(IEventLogOnTarget target);
        bool NewDataAvailiable();
        void SendData();
    }
}

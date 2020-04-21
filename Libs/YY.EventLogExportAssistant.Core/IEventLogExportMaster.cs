
namespace YY.EventLogExportAssistant
{
    public interface IEventLogExportMaster<T> where T : CommonLogObject
    {
        void AddItem(T item);

        void Send();
    }
}

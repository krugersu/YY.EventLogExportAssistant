using System;

namespace YY.EventLogExportAssistant
{
    public sealed class EventLogExportMaster<T> : IEventLogExportMaster<T>, IDisposable where T : CommonLogObject
    {
        public static EventLogExportMaster<T> CreateExportMaster(EventLogOnTarget<T> target)
        {
            return new EventLogExportMaster<T>(target);
        }

        private IEventLogTarget<T> _target;

        private EventLogExportMaster()
        {
        }
        private EventLogExportMaster(EventLogOnTarget<T> target)
        {
            _target = (IEventLogTarget<T>)target;
        }

        public void AddItem(T item)
        {
            Console.WriteLine("Item added");
        }

        public void Send()
        {
            Console.WriteLine("Sended");
        }

        public int QueueLength
        {
            get { return 0; }
        }

        public void Dispose()
        {
            
        }
    }
}

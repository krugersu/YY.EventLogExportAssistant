using System;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportAssistant
{
    public sealed class EventLogExportMaster : IEventLogExportMaster, IDisposable
    {
        public static EventLogExportMaster CreateExportMaster(EventLogTargetDefinition target)
        {
            return new EventLogExportMaster(target);
        }

        private IEventLogTargetDefinition _target;

        private EventLogExportMaster()
        {
        }
        private EventLogExportMaster(EventLogTargetDefinition target)
        {
            _target = target;
        }

        public void AddItem(object item)
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

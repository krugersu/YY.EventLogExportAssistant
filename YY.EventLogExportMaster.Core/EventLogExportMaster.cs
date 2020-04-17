using System;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportMaster
{
    public abstract class EventLogExportMaster : IEventLogExportMaster
    {
        private IEventLogTargetDefinition _target;

        private EventLogExportMaster()
        {
        }
        private EventLogExportMaster(EventLogTargetDefinition target)
        {
            _target = target;
        }
    }
}

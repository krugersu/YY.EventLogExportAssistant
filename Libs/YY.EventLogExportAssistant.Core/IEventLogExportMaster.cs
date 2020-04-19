using System;
using System.Collections.Generic;
using System.Text;

namespace YY.EventLogExportAssistant
{
    public interface IEventLogExportMaster
    {
        void AddItem(object item);

        void Send();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace YY.EventLogExportAssistant.SQLServer.Models
{
    public abstract class LogObject
    {
        public virtual long InformationSystemId { get; set; }
        public virtual InformationSystems InformationSystem { get; set; }
    }
}

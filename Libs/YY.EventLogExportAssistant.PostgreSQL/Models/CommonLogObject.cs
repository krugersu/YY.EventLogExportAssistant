using System;
using System.Collections.Generic;
using System.Text;

namespace YY.EventLogExportAssistant.PostgreSQL.Models
{
    public class CommonLogObject
    {
        public long InformationSystemId { get; set; }
        public virtual InformationSystemsBase InformationSystem { get; set; }
    }
}

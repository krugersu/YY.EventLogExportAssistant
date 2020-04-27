using System;
using System.Collections.Generic;
using System.Text;

namespace YY.EventLogExportAssistant.SQLServer.Models
{
    public class LogFiles
    {
        public long Id { set; get; }
        public string FileName { set; get; }
        public DateTime CreateDate { set; get; }
        public DateTime ModificationDate { set; get; }

    }
}

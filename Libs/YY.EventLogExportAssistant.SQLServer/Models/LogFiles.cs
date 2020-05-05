using System;
using System.Collections.Generic;
using System.Text;

namespace YY.EventLogExportAssistant.SQLServer.Models
{
    public class LogFiles : CommonLogObject
    {
        public long Id { set; get; }
        public string FileName { set; get; }
        public DateTime CreateDate { set; get; }
        public DateTime ModificationDate { set; get; }
        public long LastEventNumber { set; get; }
        public string LastCurrentFileReferences { set; get; }
        public string LastCurrentFileData { set; get; }
        public long LastStreamPosition { set; get; }

    }
}

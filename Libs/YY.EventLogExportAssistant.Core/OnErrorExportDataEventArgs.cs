using System;
using System.Collections.Generic;
using System.Text;

namespace YY.EventLogExportAssistant
{
    public class OnErrorExportDataEventArgs
    {
        public Exception Exception { get; set; }
        public string SourceData { get; set; }
        public bool Critical { get; set; }
    }
}

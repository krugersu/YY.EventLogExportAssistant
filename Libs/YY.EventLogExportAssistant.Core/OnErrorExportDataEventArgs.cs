using System;

namespace YY.EventLogExportAssistant
{
    public class OnErrorExportDataEventArgs
    {
        public Exception Exception { get; set; }
        public string SourceData { get; set; }
        public bool Critical { get; set; }
    }
}

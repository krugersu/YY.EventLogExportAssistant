using System.Collections.Generic;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportAssistant
{
    public class BeforeExportDataEventArgs
    {
        public BeforeExportDataEventArgs()
        {
            Cancel = false;
        }

        public IReadOnlyList<RowData> Rows { set; get; }
        public bool Cancel { set; get; }
    }
}

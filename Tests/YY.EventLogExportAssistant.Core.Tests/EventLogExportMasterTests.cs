using System;
using Xunit;

namespace YY.EventLogExportAssistant.Core.Tests
{
    public class EventLogExportMasterTests
    {
        [Fact]
        public void CreateEventLogExportMasterObject()
        {
            EventLogExportMaster exporter = new EventLogExportMaster();
            exporter.SetEventLogPath(string.Empty);
            exporter.SetTarget(null);
            exporter.NewDataAvailiable();
            exporter.SendData();
        }
    }
}

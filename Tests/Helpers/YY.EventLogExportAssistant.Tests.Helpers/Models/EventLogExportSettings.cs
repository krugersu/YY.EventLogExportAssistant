using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace YY.EventLogExportAssistant.Tests.Helpers.Models
{
    public class EventLogExportSettings
    {
        #region Public Members

        public string EventLogPath { set; get; }
        public int Portion { set; get; }
        public string InforamtionSystemName { set; get; }
        public string InforamtionSystemDescription { set; get; }

        #endregion

        #region Constructors

        public EventLogExportSettings()
        {
        }
        public EventLogExportSettings(IConfigurationSection configSection)
        {
            IConfigurationSection eventLogSectionLGD = configSection.GetSection("EventLog");
            EventLogPath = eventLogSectionLGD.GetValue("SourcePath", string.Empty);
            if (!Directory.Exists(EventLogPath))
            {
                List<string> pathParts = EventLogPath.Split('\\').ToList();
                pathParts.Insert(0, Directory.GetCurrentDirectory());
                EventLogPath = Path.Combine(pathParts.ToArray());
            }
            Portion = eventLogSectionLGD.GetValue("Portion", 1000);

            IConfigurationSection inforamtionSystemSectionLGD = configSection.GetSection("InformationSystem");
            InforamtionSystemName = inforamtionSystemSectionLGD.GetValue("Name", string.Empty);
            InforamtionSystemDescription = inforamtionSystemSectionLGD.GetValue("Description", string.Empty);
        }

        #endregion
    }
}

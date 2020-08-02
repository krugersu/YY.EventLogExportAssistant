using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace YY.EventLogExportAssistant.ElasticSearch.Tests.Models
{
    public class EventLogExportSettingsForElasticSearch
    {
        #region Public Members

        public string EventLogPath { set; get; }
        public int Portion { set; get; }
        public string InforamtionSystemName { set; get; }
        public string InforamtionSystemDescription { set; get; }
        public Uri NodeAddress { set; get; }
        public string IndexName { set; get; }
        public string IndexSeparation { set; get; }
        public int MaximumRetries { set; get; }
        public int MaxRetryTimeout { set; get; }

        #endregion

        #region Constructors

        public EventLogExportSettingsForElasticSearch()
        {
        }
        public EventLogExportSettingsForElasticSearch(IConfigurationSection configSection)
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

            IConfigurationSection elasticSearchSection = configSection.GetSection("ElasticSearch");
            NodeAddress = elasticSearchSection.GetValue<Uri>("Node");
            IndexName = elasticSearchSection.GetValue<string>("IndexName").ToLower();
            IndexSeparation = elasticSearchSection.GetValue<string>("IndexSeparationPeriod");
            MaximumRetries = elasticSearchSection.GetValue<int>("MaximumRetries");
            MaxRetryTimeout = elasticSearchSection.GetValue<int>("MaxRetryTimeout");
        }

        #endregion
    }
}

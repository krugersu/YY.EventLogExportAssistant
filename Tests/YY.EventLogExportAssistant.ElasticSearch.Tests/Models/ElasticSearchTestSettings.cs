using Microsoft.Extensions.Configuration;
using YY.EventLogExportAssistant.Database;

namespace YY.EventLogExportAssistant.ElasticSearch.Tests.Models
{
    public class ElasticSearchTestSettings
    {
        #region Public Members

        public EventLogExportSettingsForElasticSearch SettingsLGF { set; get; }
        public EventLogExportSettingsForElasticSearch SettingsLGD { set; get; }

        #endregion

        #region Constructors

        public ElasticSearchTestSettings()
        {
        }
        public ElasticSearchTestSettings(string configFilePath)
        {

            IConfiguration Configuration = new ConfigurationBuilder()
                .AddJsonFile(configFilePath, optional: true, reloadOnChange: true)
                .Build();
            SettingsLGF = new EventLogExportSettingsForElasticSearch(Configuration.GetSection("LGF"));
            SettingsLGD = new EventLogExportSettingsForElasticSearch(Configuration.GetSection("LGD"));
        }

        #endregion
    }
}

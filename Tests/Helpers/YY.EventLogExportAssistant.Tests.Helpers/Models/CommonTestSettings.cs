using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using YY.EventLogExportAssistant.Database;

namespace YY.EventLogExportAssistant.Tests.Helpers.Models
{
    public class CommonTestSettings
    {
        #region Public Members

        public string ConnectionString { set; get; }
        public IEventLogContextExtensionActions DBMSActions { set; get; }
        public EventLogExportSettings SettingsLGF { set; get; }
        public EventLogExportSettings SettingsLGD { set; get; }

        #endregion

        #region Constructors

        public CommonTestSettings()
        {
        }
        public CommonTestSettings(string configFilePath, IEventLogContextExtensionActions actions)
        {
            DBMSActions = actions;

            IConfiguration Configuration = new ConfigurationBuilder()
                .AddJsonFile(configFilePath, optional: true, reloadOnChange: true)
                .Build();
            ConnectionString = Configuration.GetConnectionString("EventLogDatabase");
            SettingsLGF = new EventLogExportSettings(Configuration.GetSection("LGF"));
            SettingsLGD = new EventLogExportSettings(Configuration.GetSection("LGD"));
        }

        #endregion
    }
}

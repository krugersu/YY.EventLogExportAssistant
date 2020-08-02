using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using Xunit;
using YY.EventLogExportAssistant.Database;
using YY.EventLogExportAssistant.Tests.Helpers;
using YY.EventLogExportAssistant.Tests.Helpers.Models;
using YY.EventLogReaderAssistant;

namespace YY.EventLogExportAssistant.PostgreSQL.Tests
{
    [CollectionDefinition("YY.EventLogExportAssistant.PostgreSQL", DisableParallelization = true)]
    public class EventLogExportMasterTests
    {
        #region Private Member Variables

        private readonly CommonTestSettings _settings;
        DbContextOptionsBuilder<EventLogContext> _optionsBuilder;

        #endregion

        #region Constructors

        public EventLogExportMasterTests()
        {
            string configFilePath = GetConfigFile();

            _settings = new CommonTestSettings(
                configFilePath,
                new EventLogPostgreSQLActions());

            _optionsBuilder = new DbContextOptionsBuilder<EventLogContext>();
            _optionsBuilder.UseNpgsql(_settings.ConnectionString);

            using (EventLogContext context = EventLogContext.Create(_optionsBuilder.Options, _settings.DBMSActions))
                context.Database.EnsureDeleted();
        }

        #endregion

        #region Public Methods

        [Fact]
        public void ExportFormatLGFToSQLServerTest()
        {
            ExportToPostgreSQL(_settings.SettingsLGF);
        }
        [Fact]
        public void ExportFormatLGDToSQLServerTest()
        {
            ExportToPostgreSQL(_settings.SettingsLGD);
        }

        #endregion

        #region Private Methods

        private void ExportToPostgreSQL(EventLogExportSettings eventLogSettings)
        {
            EventLogOnPostgreSQL target = new EventLogOnPostgreSQL(_optionsBuilder.Options, eventLogSettings.Portion);
            target.SetInformationSystem(new InformationSystemsBase()
            {
                Name = eventLogSettings.InforamtionSystemName,
                Description = eventLogSettings.InforamtionSystemDescription
            });

            ExportHelper.ExportToTargetStorage(eventLogSettings, target);

            long rowsInDB;
            using (EventLogContext context = EventLogContext.Create(_optionsBuilder.Options, _settings.DBMSActions))
            {
                var informationSystem = context.InformationSystems
                    .First(i => i.Name == eventLogSettings.InforamtionSystemName);
                var getCount = context.RowsData
                    .Where(r => r.InformationSystemId == informationSystem.Id)
                    .LongCountAsync();
                getCount.Wait();
                rowsInDB = getCount.Result;
            }

            long rowsInSourceFiles;
            using (EventLogReader reader = EventLogReader.CreateReader(eventLogSettings.EventLogPath))
                rowsInSourceFiles = reader.Count();

            Assert.NotEqual(0, rowsInSourceFiles);
            Assert.NotEqual(0, rowsInDB);
            Assert.Equal(rowsInSourceFiles, rowsInDB);
        }
        private string GetConfigFile()
        {
            // TODO
            // Перенести формирование конфигурационного файла в скрипты CI

            string configFilePath = "appsettings.json";
            if (!File.Exists(configFilePath))
            {
                configFilePath = "travisci-appsettings.json";
                IConfiguration Configuration = new ConfigurationBuilder()
                    .AddJsonFile(configFilePath, optional: true, reloadOnChange: true)
                    .Build();
                string connectionString = Configuration.GetConnectionString("EventLogDatabase");
                try
                {
                    _optionsBuilder = new DbContextOptionsBuilder<EventLogContext>();
                    _optionsBuilder.UseNpgsql(connectionString);
                    using (EventLogContext context = EventLogContext.Create(_optionsBuilder.Options, new EventLogPostgreSQLActions()))
                        context.Database.EnsureDeleted();
                }
                catch
                {
                    configFilePath = "appveyor-appsettings.json";
                }
            }

            if (!File.Exists(configFilePath))
                throw new Exception("Файл конфигурации не обнаружен.");

            return configFilePath;
        }

        #endregion
    }
}

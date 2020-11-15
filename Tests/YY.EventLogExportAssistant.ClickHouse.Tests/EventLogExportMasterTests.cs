using System;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using ClickHouse.Client.ADO;
using ClickHouse.Client.ADO.Parameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;
using YY.EventLogExportAssistant.ClickHouse.Helpers;
using YY.EventLogExportAssistant.Database;
using YY.EventLogExportAssistant.Tests.Helpers;
using YY.EventLogExportAssistant.Tests.Helpers.Models;
using YY.EventLogReaderAssistant;

namespace YY.EventLogExportAssistant.ClickHouse.Tests
{
    [CollectionDefinition("YY.EventLogExportAssistant.ClickHouse", DisableParallelization = true)]
    public class EventLogExportMasterTests
    {
        #region Private Member Variables

        private readonly CommonTestSettings _settings;

        #endregion

        #region Constructors

        public EventLogExportMasterTests()
        {
            string configFilePath = GetConfigFile();
            Console.WriteLine($"Config file: {configFilePath}");

            _settings = new CommonTestSettings(configFilePath, null);
            Console.WriteLine($"Connection string: {_settings.ConnectionString}");
        }

        #endregion

        #region Public Methods

        [Fact]
        public void ExportFormatLGFToClickHouseTest()
        {
            ExportToClickHouse(_settings.SettingsLGF);
        }
        [Fact]
        public void ExportFormatLGDToClickHouseTest()
        {
            ExportToClickHouse(_settings.SettingsLGD);
        }

        #endregion

        #region Private Methods

        private void ExportToClickHouse(EventLogExportSettings eventLogSettings)
        {
            ClickHouseHelpers.DropDatabaseIfNotExist(_settings.ConnectionString);

            EventLogOnClickHouse target = new EventLogOnClickHouse(_settings.ConnectionString, eventLogSettings.Portion);
            target.SetInformationSystem(new InformationSystemsBase()
            {
                Name = eventLogSettings.InforamtionSystemName,
                Description = eventLogSettings.InforamtionSystemDescription
            });
            
            ExportHelper.ExportToTargetStorage(eventLogSettings, target);

            long rowsInDB;
            using (var connection = new ClickHouseConnection(_settings.ConnectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText =
                        @"SELECT 
                            COUNT(*) CNT 
                        FROM RowsData rd
                        WHERE InformationSystem = {InformationSystem:String}";
                    cmd.Parameters.Add(new ClickHouseDbParameter
                    {
                        ParameterName = "InformationSystem",
                        Value = eventLogSettings.InforamtionSystemName
                    });
                    using (var cmdReader = cmd.ExecuteReader())
                    {
                        if (cmdReader.Read())
                            rowsInDB = cmdReader.GetInt64(0);
                        else
                            rowsInDB = 0;
                    }
                }
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
                // Для AppVeyor тест пока не запускаем
                //try
                //{
                //    using (var context = new ClickHouseContext(connectionString))
                //    {
                //        var someValue = context.GetHashCode();
                //    }
                //}
                //catch
                //{
                //    configFilePath = "appveyor-appsettings.json";
                //}
            }

            if (!File.Exists(configFilePath))
                throw new Exception("Файл конфигурации не обнаружен.");

            return configFilePath;
        }

        #endregion
    }
}

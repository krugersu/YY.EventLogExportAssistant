using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using YY.EventLogReaderAssistant;

namespace YY.EventLogExportAssistant.SQLServer.Tests
{
    [CollectionDefinition("YY.EventLogExportAssistant.SQLServer.LGF", DisableParallelization = true)]
    public class EventLogExportMaster_LGF_Tests
    {
        #region Private Member Variables

        string eventLogPath;
        int portion;
        string inforamtionSystemName;
        string inforamtionSystemDescription;
        string connectionString;

        DbContextOptionsBuilder<EventLogContext> optionsBuilder;

        #endregion

        public EventLogExportMaster_LGF_Tests()
        {
            string configFilePath = GetConfigFile();

            if (!File.Exists(configFilePath))
                throw new Exception("Файл конфигурации не обнаружен.");

            IConfiguration Configuration = new ConfigurationBuilder()
                .AddJsonFile(configFilePath, optional: true, reloadOnChange: true)
                .Build();

            IConfigurationSection LGFSection = Configuration.GetSection("LGF");

            IConfigurationSection eventLogSection = LGFSection.GetSection("EventLog");
            eventLogPath = eventLogSection.GetValue("SourcePath", string.Empty);
            if (!Directory.Exists(eventLogPath))
            {
                List<string> pathParts = eventLogPath.Split('\\', StringSplitOptions.RemoveEmptyEntries).ToList();
                pathParts.Insert(0, Directory.GetCurrentDirectory());
                eventLogPath = Path.Combine(pathParts.ToArray());
            }
            eventLogSection.GetValue("WatchPeriod", 60);
            eventLogSection.GetValue("UseWatchMode", false);
            portion = eventLogSection.GetValue("Portion", 1000);

            IConfigurationSection inforamtionSystemSection = LGFSection.GetSection("InformationSystem");
            inforamtionSystemName = inforamtionSystemSection.GetValue("Name", string.Empty);
            inforamtionSystemDescription = inforamtionSystemSection.GetValue("Description", string.Empty);

            IConfigurationSection connectionStringsSection = LGFSection.GetSection("ConnectionStrings");
            connectionString = connectionStringsSection.GetValue("EventLogDatabase", string.Empty);

            optionsBuilder = new DbContextOptionsBuilder<EventLogContext>();
            optionsBuilder.UseSqlServer(connectionString);
            using (EventLogContext context = new EventLogContext(optionsBuilder.Options))            
                context.Database.EnsureDeleted();
        }

        [Fact]
        public void ExportToSQLServer_LGF_Test()
        {
            if (!Directory.Exists(eventLogPath))
                throw new Exception("Каталог данных журнала регистрации не обнаружен.");

            EventLogExportMaster exporter = new EventLogExportMaster();
            exporter.SetEventLogPath(eventLogPath);

            EventLogOnSQLServer target = new EventLogOnSQLServer(optionsBuilder.Options, portion);
            target.SetInformationSystem(new InformationSystemsBase()
            {
                Name = inforamtionSystemName,
                Description = inforamtionSystemDescription
            });
            exporter.SetTarget(target);

            exporter.BeforeExportData += BeforeExportData;
            exporter.AfterExportData += AfterExportData;
            exporter.OnErrorExportData += OnErrorExportData;

            while (exporter.NewDataAvailiable())
                exporter.SendData();

            long rowsInDB;
            using(EventLogContext context = new EventLogContext(optionsBuilder.Options))
            {
                var getCount = context.RowsData.LongCountAsync();
                getCount.Wait();
                rowsInDB = getCount.Result;
            }

            long rowsInSourceFiles;
            using (EventLogReader reader = EventLogReader.CreateReader(eventLogPath))
            {
                rowsInSourceFiles = reader.Count();
            }

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
                connectionString = Configuration.GetConnectionString("EventLogDatabase");
                try
                {
                    optionsBuilder = new DbContextOptionsBuilder<EventLogContext>();
                    optionsBuilder.UseSqlServer(connectionString);
                    using (EventLogContext context = new EventLogContext(optionsBuilder.Options))
                        context.Database.EnsureDeleted();
                }
                catch
                {
                    configFilePath = "appveyor-appsettings.json";
                }
            }

            return configFilePath;
        }

        #region Events

        private static void BeforeExportData(BeforeExportDataEventArgs e)
        {
        }
        private static void AfterExportData(AfterExportDataEventArgs e)
        {
        }
        private static void OnErrorExportData(OnErrorExportDataEventArgs e)
        {
            throw e.Exception;
        }

        #endregion
    }
}

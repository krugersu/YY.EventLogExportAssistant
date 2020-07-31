using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using YY.EventLogExportAssistant.Database;
using YY.EventLogReaderAssistant;

namespace YY.EventLogExportAssistant.SQLServer.Tests
{
    [CollectionDefinition("YY.EventLogExportAssistant.SQLServer", DisableParallelization = true)]
    public class EventLogExportMasterTests
    {
        #region Private Member Variables

        string connectionString;
        DbContextOptionsBuilder<EventLogContext> optionsBuilder;
        private IEventLogContextExtensionActions _SqlServerActions;

        #region LGF Settings

        string eventLogPathLGF;
        int portionLGF;
        string inforamtionSystemNameLGF;
        string inforamtionSystemDescriptionLGF;

        #endregion

        #region LGD Settings

        string eventLogPathLGD;
        int portionLGD;
        string inforamtionSystemNameLGD;
        string inforamtionSystemDescriptionLGD;

        #endregion

        #endregion

        #region Constructors

        public EventLogExportMasterTests()
        {
            _SqlServerActions = new EventLogSQLServerActions();
            string configFilePath = GetConfigFile();

            if (!File.Exists(configFilePath))
                throw new Exception("Файл конфигурации не обнаружен.");

            IConfiguration Configuration = new ConfigurationBuilder()
                .AddJsonFile(configFilePath, optional: true, reloadOnChange: true)
                .Build();

            #region Database Settings

            connectionString = Configuration.GetConnectionString("EventLogDatabase");
            optionsBuilder = new DbContextOptionsBuilder<EventLogContext>();
            optionsBuilder.UseSqlServer(connectionString);
            using (EventLogContext context = new EventLogContext(optionsBuilder.Options,
                _SqlServerActions, DBMSType.SQLServer))
                context.Database.EnsureDeleted();

            #endregion

            IConfigurationSection LGFSection = Configuration.GetSection("LGF");
            #region LGF Format

            IConfigurationSection eventLogSectionLGF = LGFSection.GetSection("EventLog");
            eventLogPathLGF = eventLogSectionLGF.GetValue("SourcePath", string.Empty);
            if (!Directory.Exists(eventLogPathLGF))
            {
                List<string> pathParts = eventLogPathLGF.Split('\\', StringSplitOptions.RemoveEmptyEntries).ToList();
                pathParts.Insert(0, Directory.GetCurrentDirectory());
                eventLogPathLGF = Path.Combine(pathParts.ToArray());
            }
            eventLogSectionLGF.GetValue("WatchPeriod", 60);
            eventLogSectionLGF.GetValue("UseWatchMode", false);
            portionLGF = eventLogSectionLGF.GetValue("Portion", 1000);

            IConfigurationSection inforamtionSystemSectionLGF = LGFSection.GetSection("InformationSystem");
            inforamtionSystemNameLGF = inforamtionSystemSectionLGF.GetValue("Name", string.Empty);
            inforamtionSystemDescriptionLGF = inforamtionSystemSectionLGF.GetValue("Description", string.Empty);

            #endregion

            IConfigurationSection LGDSection = Configuration.GetSection("LGD");
            #region LGF Format

            IConfigurationSection eventLogSectionLGD = LGDSection.GetSection("EventLog");
            eventLogPathLGD = eventLogSectionLGD.GetValue("SourcePath", string.Empty);
            if (!Directory.Exists(eventLogPathLGD))
            {
                List<string> pathParts = eventLogPathLGD.Split('\\', StringSplitOptions.RemoveEmptyEntries).ToList();
                pathParts.Insert(0, Directory.GetCurrentDirectory());
                eventLogPathLGD = Path.Combine(pathParts.ToArray());
            }
            eventLogSectionLGD.GetValue("WatchPeriod", 60);
            eventLogSectionLGD.GetValue("UseWatchMode", false);
            portionLGD = eventLogSectionLGD.GetValue("Portion", 1000);

            IConfigurationSection inforamtionSystemSectionLGD = LGDSection.GetSection("InformationSystem");
            inforamtionSystemNameLGD = inforamtionSystemSectionLGD.GetValue("Name", string.Empty);
            inforamtionSystemDescriptionLGD = inforamtionSystemSectionLGD.GetValue("Description", string.Empty);

            #endregion
        }

        #endregion

        #region Public Methods

        [Fact]
        public void ExportToSQLServerTest()
        {
            ExportToSQLServer_LGF_Test();

            ExportToSQLServer_LGD_Test();

            long informationSystemsCount;
            using (EventLogContext context = new EventLogContext(optionsBuilder.Options,
                _SqlServerActions, DBMSType.SQLServer))
                informationSystemsCount = context.InformationSystems.Count();

            Assert.Equal(2, informationSystemsCount);
        }

        #endregion

        #region Private Methods

        private void ExportToSQLServer_LGF_Test()
        {
            if (!Directory.Exists(eventLogPathLGF))
                throw new Exception("Каталог данных журнала регистрации не обнаружен.");

            EventLogExportMaster exporter = new EventLogExportMaster();
            exporter.SetEventLogPath(eventLogPathLGF);

            EventLogOnSQLServer target = new EventLogOnSQLServer(optionsBuilder.Options, portionLGF);
            target.SetInformationSystem(new InformationSystemsBase()
            {
                Name = inforamtionSystemNameLGF,
                Description = inforamtionSystemDescriptionLGF
            });
            exporter.SetTarget(target);

            exporter.BeforeExportData += BeforeExportData;
            exporter.AfterExportData += AfterExportData;
            exporter.OnErrorExportData += OnErrorExportData;

            while (exporter.NewDataAvailiable())
                exporter.SendData();

            long rowsInDB;
            using (EventLogContext context = new EventLogContext(optionsBuilder.Options,
                _SqlServerActions, DBMSType.SQLServer))
            {
                var informationSystem = context.InformationSystems
                    .First(i => i.Name == inforamtionSystemNameLGF);
                var getCount = context.RowsData
                    .Where(r => r.InformationSystemId == informationSystem.Id)
                    .LongCountAsync();
                getCount.Wait();
                rowsInDB = getCount.Result;
            }

            long rowsInSourceFiles;
            using (EventLogReader reader = EventLogReader.CreateReader(eventLogPathLGF))
            {
                rowsInSourceFiles = reader.Count();
            }

            Assert.NotEqual(0, rowsInSourceFiles);
            Assert.NotEqual(0, rowsInDB);
            Assert.Equal(rowsInSourceFiles, rowsInDB);
        }
        private void ExportToSQLServer_LGD_Test()
        {
            if (!Directory.Exists(eventLogPathLGD))
                throw new Exception("Каталог данных журнала регистрации не обнаружен.");

            EventLogExportMaster exporter = new EventLogExportMaster();
            exporter.SetEventLogPath(eventLogPathLGD);

            EventLogOnSQLServer target = new EventLogOnSQLServer(optionsBuilder.Options, portionLGD);
            target.SetInformationSystem(new InformationSystemsBase()
            {
                Name = inforamtionSystemNameLGD,
                Description = inforamtionSystemDescriptionLGD
            });
            exporter.SetTarget(target);

            exporter.BeforeExportData += BeforeExportData;
            exporter.AfterExportData += AfterExportData;
            exporter.OnErrorExportData += OnErrorExportData;

            while (exporter.NewDataAvailiable())
                exporter.SendData();

            long rowsInDB;
            using (EventLogContext context = new EventLogContext(optionsBuilder.Options,
                _SqlServerActions, DBMSType.SQLServer))
            {
                var informationSystem = context.InformationSystems
                    .First(i => i.Name == inforamtionSystemNameLGD);
                var getCount = context.RowsData
                    .Where(r => r.InformationSystemId == informationSystem.Id)
                    .LongCountAsync();
                getCount.Wait();
                rowsInDB = getCount.Result;
            }

            long rowsInSourceFiles;
            using (EventLogReader reader = EventLogReader.CreateReader(eventLogPathLGD))
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
                    using (EventLogContext context = new EventLogContext(optionsBuilder.Options,
                        _SqlServerActions, DBMSType.SQLServer))
                        context.Database.EnsureDeleted();
                }
                catch
                {
                    configFilePath = "appveyor-appsettings.json";
                }
            }

            return configFilePath;
        }

        #endregion

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

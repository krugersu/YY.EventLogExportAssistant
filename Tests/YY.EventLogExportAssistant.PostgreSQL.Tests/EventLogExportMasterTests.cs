using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Xunit;
using YY.EventLogReaderAssistant;

namespace YY.EventLogExportAssistant.PostgreSQL.Tests
{
    public class EventLogExportMasterTests
    {
        #region Private Static Member Variables

        private static long _totalRows = 0;
        private static long _lastPortionRows = 0;
        private static DateTime _beginPortionExport;
        private static DateTime _endPortionExport;

        string eventLogPath;
        int watchPeriodSeconds;
        int watchPeriodSecondsMs;
        bool useWatchMode;
        int portion;
        string inforamtionSystemName;
        string inforamtionSystemDescription;
        string connectionString;

        DbContextOptionsBuilder<EventLogContext> optionsBuilder;

        #endregion

        public EventLogExportMasterTests()
        {
            string configFilePath = "appsettings.json";
            if (!File.Exists(configFilePath))
                configFilePath = "appveyor-appsettings.json";

            if (!File.Exists(configFilePath))
                throw new Exception("Файл конфигурации не обнаружен.");

            IConfiguration Configuration = new ConfigurationBuilder()
                .AddJsonFile(configFilePath, optional: true, reloadOnChange: true)
                .Build();

            IConfigurationSection eventLogSection = Configuration.GetSection("EventLog");
            eventLogPath = eventLogSection.GetValue("SourcePath", string.Empty);
            watchPeriodSeconds = eventLogSection.GetValue("WatchPeriod", 60);
            watchPeriodSecondsMs = watchPeriodSeconds * 1000;
            useWatchMode = eventLogSection.GetValue("UseWatchMode", false);
            portion = eventLogSection.GetValue("Portion", 1000);

            IConfigurationSection inforamtionSystemSection = Configuration.GetSection("InformationSystem");
            inforamtionSystemName = inforamtionSystemSection.GetValue("Name", string.Empty);
            inforamtionSystemDescription = inforamtionSystemSection.GetValue("Description", string.Empty);

            connectionString = Configuration.GetConnectionString("EventLogDatabase");

            optionsBuilder = new DbContextOptionsBuilder<EventLogContext>();
            optionsBuilder.UseNpgsql(connectionString);
            using (EventLogContext context = new EventLogContext(optionsBuilder.Options))
                context.Database.EnsureDeleted();
        }

        [Fact]
        public void ExportToSQLServerTest()
        {
            if (!Directory.Exists(eventLogPath))
                throw new Exception("Каталог данных журнала регистрации не обнаружен.");

            EventLogExportMaster exporter = new EventLogExportMaster();
            exporter.SetEventLogPath(eventLogPath);

            EventLogOnPostgreSQL target = new EventLogOnPostgreSQL(optionsBuilder.Options, portion);
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

            long rowsInDB = 0;
            using (EventLogContext context = new EventLogContext(optionsBuilder.Options))
            {
                var getCount = context.RowsData.LongCountAsync();
                getCount.Wait();
                rowsInDB = getCount.Result;
            }

            long rowsInSourceFiles = 0;
            using (EventLogReader reader = EventLogReader.CreateReader(eventLogPath))
            {
                rowsInSourceFiles = reader.Count();
            }

            Assert.NotEqual(0, rowsInSourceFiles);
            Assert.NotEqual(0, rowsInDB);
            Assert.Equal(rowsInSourceFiles, rowsInDB);
        }

        #region Events

        private static void BeforeExportData(BeforeExportDataEventArgs e)
        {
            _beginPortionExport = DateTime.Now;
            _lastPortionRows = e.Rows.Count;
            _totalRows += e.Rows.Count;
        }
        private static void AfterExportData(AfterExportDataEventArgs e)
        {
            _endPortionExport = DateTime.Now;
            var duration = _endPortionExport - _beginPortionExport;
        }
        private static void OnErrorExportData(OnErrorExportDataEventArgs e)
        {
            throw e.Exception;
        }

        #endregion
    }
}

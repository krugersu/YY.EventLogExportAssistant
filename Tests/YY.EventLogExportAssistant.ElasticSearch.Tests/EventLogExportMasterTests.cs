using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Nest;
using Xunit;
using YY.EventLogExportAssistant.ElasticSearch.Models;
using YY.EventLogReaderAssistant;

namespace YY.EventLogExportAssistant.ElasticSearch.Tests
{
    public class EventLogExportMasterTests
    {
        #region Private Member Variables

        #region LGF Settings

        private string eventLogPathLGF;
        private int portionLGF;
        private string inforamtionSystemNameLGF;
        private string inforamtionSystemDescriptionLGF;

        private Uri _nodeAddressLGF;
        private string _indexNameLGF;
        private string _indexSeparationLGF;
        private int _maximumRetriesLGF;
        private int _maxRetryTimeoutLGF;

        #endregion

        #region LGD Settings

        private string eventLogPathLGD;
        private int portionLGD;
        private string inforamtionSystemNameLGD;
        private string inforamtionSystemDescriptionLGD;

        private Uri _nodeAddressLGD;
        private string _indexNameLGD;
        private string _indexSeparationLGD;
        private int _maximumRetriesLGD;
        private int _maxRetryTimeoutLGD;

        #endregion

        #endregion

        #region Constructors

        public EventLogExportMasterTests()
        {
            string configFilePath = GetConfigFile();

            if (!File.Exists(configFilePath))
                throw new Exception("Файл конфигурации не обнаружен.");

            IConfiguration Configuration = new ConfigurationBuilder()
                .AddJsonFile(configFilePath, optional: true, reloadOnChange: true)
                .Build();

            #region ElasticSearch Settings

            #region LGF Format

            IConfigurationSection LGFSection = Configuration.GetSection("LGF");

            IConfigurationSection elasticSearchSectionLGF = LGFSection.GetSection("ElasticSearch");
            _nodeAddressLGF = elasticSearchSectionLGF.GetValue<Uri>("Node");
            _indexNameLGF = elasticSearchSectionLGF.GetValue<string>("IndexName");
            _indexNameLGF = _indexNameLGF.ToLower();
            _indexSeparationLGF = elasticSearchSectionLGF.GetValue<string>("IndexSeparationPeriod");
            _maximumRetriesLGF = elasticSearchSectionLGF.GetValue<int>("MaximumRetries");
            _maxRetryTimeoutLGF = elasticSearchSectionLGF.GetValue<int>("MaxRetryTimeout");

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

            #region LGD Format

            IConfigurationSection LGDSection = Configuration.GetSection("LGD");

            IConfigurationSection elasticSearchSectionLGD = LGDSection.GetSection("ElasticSearch");
            _nodeAddressLGD = elasticSearchSectionLGD.GetValue<Uri>("Node");
            _indexNameLGD = elasticSearchSectionLGD.GetValue<string>("IndexName");
            _indexNameLGD = _indexNameLGD.ToLower();
            _indexSeparationLGD = elasticSearchSectionLGD.GetValue<string>("IndexSeparationPeriod");
            _maximumRetriesLGD = elasticSearchSectionLGD.GetValue<int>("MaximumRetries");
            _maxRetryTimeoutLGD = elasticSearchSectionLGD.GetValue<int>("MaxRetryTimeout");

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

            #endregion
        }

        #endregion

        #region Public Methods

        [Fact]
        public void ExportToElasticSearchTest()
        {
            ExportToElasticSearch_LGF_Test();

            ExportToElasticSearch_LGD_Test();
        }

        #endregion

        #region Private Methods

        private void ExportToElasticSearch_LGF_Test()
        {
            if (!Directory.Exists(eventLogPathLGF))
                throw new Exception("Каталог данных журнала регистрации не обнаружен.");

            EventLogExportMaster exporter = new EventLogExportMaster();
            exporter.SetEventLogPath(eventLogPathLGF);

            ConnectionSettings elasticSettings = new ConnectionSettings(_nodeAddressLGF)
                .DefaultIndex(_indexNameLGF)
                .MaximumRetries(_maximumRetriesLGF)
                .MaxRetryTimeout(TimeSpan.FromSeconds(_maxRetryTimeoutLGF));

            ElasticClient client = new ElasticClient(elasticSettings);

            var allIndices = client.Indices.Get(new GetIndexRequest(Indices.All));
            foreach (var indexInfo in allIndices.Indices)
            {
                if (indexInfo.Key.Name.StartsWith(_indexNameLGF))
                {
                    client.Indices.Delete(indexInfo.Key.Name);
                }
            }
            
            EventLogOnElasticSearch target = new EventLogOnElasticSearch(elasticSettings, portionLGF);
            target.SetInformationSystem(new InformationSystemsBase()
            {
                Name = inforamtionSystemNameLGF,
                Description = inforamtionSystemDescriptionLGF
            });
            target.SetIndexName(_indexNameLGF);
            target.SetIndexSeparationPeriod(_indexSeparationLGF);
            exporter.SetTarget(target);

            exporter.BeforeExportData += BeforeExportData;
            exporter.AfterExportData += AfterExportData;
            exporter.OnErrorExportData += OnErrorExportData;

            while (exporter.NewDataAvailiable())
                exporter.SendData();

            Thread.Sleep(1000);

            long rowsInES = 0;
            string indexNameLGF_WithData = $"{_indexNameLGF}-logdata";
            var allIndicesCheckData = client.Indices.Get(new GetIndexRequest(Indices.All));
            foreach (var indexInfo in allIndicesCheckData.Indices)
            {
                if (indexInfo.Key.Name.StartsWith(indexNameLGF_WithData))
                {
                    var countResponse = client.Count<LogDataElement>(c => c
                        .Index(indexInfo.Key.Name));
                    rowsInES += countResponse.Count;
                }
            }
            
            long rowsInSourceFiles;
            using (EventLogReader reader = EventLogReader.CreateReader(eventLogPathLGF))
            {
                rowsInSourceFiles = reader.Count();
            }

            Assert.NotEqual(0, rowsInSourceFiles);
            Assert.NotEqual(0, rowsInES);
            Assert.Equal(rowsInSourceFiles, rowsInES);
        }
        private void ExportToElasticSearch_LGD_Test()
        {
            if (!Directory.Exists(eventLogPathLGD))
                throw new Exception("Каталог данных журнала регистрации не обнаружен.");

            EventLogExportMaster exporter = new EventLogExportMaster();
            exporter.SetEventLogPath(eventLogPathLGD);

            ConnectionSettings elasticSettings = new ConnectionSettings(_nodeAddressLGD)
                .DefaultIndex(_indexNameLGD)
                .MaximumRetries(_maximumRetriesLGD)
                .MaxRetryTimeout(TimeSpan.FromSeconds(_maxRetryTimeoutLGD));

            ElasticClient client = new ElasticClient(elasticSettings);

            var allIndices = client.Indices.Get(new GetIndexRequest(Indices.All));
            foreach (var indexInfo in allIndices.Indices)
            {
                if (indexInfo.Key.Name.StartsWith(_indexNameLGD))
                {
                    client.Indices.Delete(indexInfo.Key.Name);
                }
            }

            EventLogOnElasticSearch target = new EventLogOnElasticSearch(elasticSettings, portionLGD);
            target.SetInformationSystem(new InformationSystemsBase()
            {
                Name = inforamtionSystemNameLGD,
                Description = inforamtionSystemDescriptionLGD
            });
            target.SetIndexName(_indexNameLGD);
            target.SetIndexSeparationPeriod(_indexSeparationLGD);
            exporter.SetTarget(target);

            exporter.BeforeExportData += BeforeExportData;
            exporter.AfterExportData += AfterExportData;
            exporter.OnErrorExportData += OnErrorExportData;

            while (exporter.NewDataAvailiable())
                exporter.SendData();

            Thread.Sleep(1000);

            long rowsInES = 0;
            string indexNameLGD_WithData = $"{_indexNameLGD}-logdata";
            var allIndicesCheckData = client.Indices.Get(new GetIndexRequest(Indices.All));
            foreach (var indexInfo in allIndicesCheckData.Indices)
            {
                if (indexInfo.Key.Name.StartsWith(indexNameLGD_WithData))
                {
                    var countResponse = client.Count<LogDataElement>(c => c
                        .Index(indexInfo.Key.Name));
                    rowsInES += countResponse.Count;
                }
            }

            long rowsInSourceFiles;
            using (EventLogReader reader = EventLogReader.CreateReader(eventLogPathLGD))
            {
                rowsInSourceFiles = reader.Count();
            }

            Assert.NotEqual(0, rowsInSourceFiles);
            Assert.NotEqual(0, rowsInES);
            Assert.Equal(rowsInSourceFiles, rowsInES);
        }
        private string GetConfigFile()
        {
            // TODO
            // Перенести формирование конфигурационного файла в скрипты CI

            string configFilePath = "appsettings.json";
            //if (!File.Exists(configFilePath))
            //{
            //    configFilePath = "travisci-appsettings";
            //    IConfiguration Configuration = new ConfigurationBuilder()
            //        .AddJsonFile(configFilePath, optional: true, reloadOnChange: true)
            //        .Build();
            //    connectionString = Configuration.GetConnectionString("EventLogDatabase");
            //    try
            //    {
            //        optionsBuilder = new DbContextOptionsBuilder<EventLogContext>();
            //        optionsBuilder.UseNpgsql(connectionString);
            //        using (EventLogContext context = new EventLogContext(optionsBuilder.Options))
            //            context.Database.EnsureDeleted();
            //    }
            //    catch
            //    {
            //        configFilePath = "appveyor-appsettings.json";
            //    }
            //}

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

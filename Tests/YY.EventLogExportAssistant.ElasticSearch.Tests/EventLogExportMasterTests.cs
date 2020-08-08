using System;
using System.IO;
using System.Threading;
using Nest;
using Xunit;
using YY.EventLogExportAssistant.ElasticSearch.Models;
using YY.EventLogExportAssistant.ElasticSearch.Tests.Models;
using YY.EventLogReaderAssistant;

namespace YY.EventLogExportAssistant.ElasticSearch.Tests
{
    public class EventLogExportMasterTests
    {
        #region Private Member Variables

        private readonly ElasticSearchTestSettings _settings;

        #endregion

        #region Constructors

        public EventLogExportMasterTests()
        {
            string configFilePath = GetConfigFile();

            _settings = new ElasticSearchTestSettings(configFilePath);
        }

        #endregion

        #region Public Methods

        [Fact]
        public void ExportFormatLGFToSQLServerTest()
        {
            ExportToElasticSearch(_settings.SettingsLGF);
        }
        [Fact]
        public void ExportFormatLGDToSQLServerTest()
        {
            ExportToElasticSearch(_settings.SettingsLGD);
        }

        #endregion

        #region Private Methods

        private void ExportToElasticSearch(EventLogExportSettingsForElasticSearch eventLogSettings)
        {
            ConnectionSettings elasticSettings = new ConnectionSettings(eventLogSettings.NodeAddress)
                .DefaultIndex(eventLogSettings.IndexName)
                .MaximumRetries(eventLogSettings.MaximumRetries)
                .MaxRetryTimeout(TimeSpan.FromSeconds(eventLogSettings.MaxRetryTimeout));

            EventLogOnElasticSearch target = new EventLogOnElasticSearch(elasticSettings, eventLogSettings.Portion);
            target.SetInformationSystem(new InformationSystemsBase()
            {
                Name = eventLogSettings.InforamtionSystemName,
                Description = eventLogSettings.InforamtionSystemDescription
            });
            target.SetIndexName(eventLogSettings.IndexName);
            target.SetIndexSeparationPeriod(eventLogSettings.IndexSeparation);

            ElasticClient client = new ElasticClient(elasticSettings);
            var allIndices = client.Indices.Get(new GetIndexRequest(Indices.All));
            foreach (var indexInfo in allIndices.Indices)
            {
                if (indexInfo.Key.Name.StartsWith(eventLogSettings.IndexName))
                    client.Indices.Delete(indexInfo.Key.Name);
            }

            ExportHelperForElasticSearch.ExportToTargetStorage(eventLogSettings, target);

            Thread.Sleep(30000);

            long rowsInES = 0;
            string indexNameLGF_WithData = $"{eventLogSettings.IndexName}-logdata";
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
            using (EventLogReader reader = EventLogReader.CreateReader(eventLogSettings.EventLogPath))
                rowsInSourceFiles = reader.Count();
            
            Assert.NotEqual(0, rowsInSourceFiles);
            Assert.NotEqual(0, rowsInES);
            Assert.Equal(rowsInSourceFiles, rowsInES);
        }
        private string GetConfigFile()
        {
            // TODO
            // Перенести формирование конфигурационного файла в скрипты CI

            string configFilePath = "appsettings.json";
            if (!File.Exists(configFilePath))
            {
                configFilePath = "ci-appsettings.json";
            }

            if (!File.Exists(configFilePath))
                throw new Exception("Файл конфигурации не обнаружен.");

            return configFilePath;
        }

        #endregion
    }
}

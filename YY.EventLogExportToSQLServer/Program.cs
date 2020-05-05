using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using YY.EventLogExportAssistant;
using YY.EventLogExportAssistant.SQLServer;
using YY.EventLogExportAssistant.SQLServer.Models;
using YY.EventLogReaderAssistant;

namespace YY.EventLogExportToSQLServer
{
    class Program
    {
        private static long _totalRows = 0;

        static void Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            IConfigurationSection eventLogSection = Configuration.GetSection("EventLog");
            string eventLogPath = eventLogSection.GetValue("SourcePath", string.Empty);
            int watchPeriodSeconds = eventLogSection.GetValue("WatchPeriod", 60);
            bool useWatchMode = eventLogSection.GetValue("UseWatchMode", false);
            int portion = eventLogSection.GetValue("Portion", 1000);

            IConfigurationSection inforamtionSystemSection = Configuration.GetSection("InformationSystem");
            string inforamtionSystemName = inforamtionSystemSection.GetValue("Name", string.Empty);
            string inforamtionSystemDescription = inforamtionSystemSection.GetValue("Description", string.Empty);

            if (string.IsNullOrEmpty(eventLogPath))
            {
                Console.WriteLine("Не указан каталог с файлами данных журнала регистрации.");
                Console.WriteLine("Для выхода нажмите любую клавишу...");
                Console.Read();
                return;
            }

            Console.WriteLine();
            Console.WriteLine();

            using (EventLogExportMaster exporter = new EventLogExportMaster())
            {
                exporter.SetEventLogPath(eventLogPath);
                exporter.SetWatchPeriod(watchPeriodSeconds);

                EventLogOnSQLServer target = new EventLogOnSQLServer(portion);
                target.SetInformationSystem(new InformationSystems()
                {
                    Name = inforamtionSystemName,
                    Description = inforamtionSystemDescription
                });
                exporter.SetTarget(target);

                exporter.BeforeExportData += BeforeExportData;
                exporter.AfterExportData += AfterExportData;

                if(useWatchMode)
                {
                    exporter.BeginWatch();

                    Console.WriteLine("Нажмите 'q' для завершения отслеживания изменений...");
                    while (Console.Read() != 'q');

                    exporter.EndWatch();
                }
                else
                {
                    while(exporter.NewDataAvailiable())
                        exporter.SendData();
                }
            }

            Console.WriteLine("Для выхода нажмите любую клавишу...");
            Console.Read();
        }

        private static void BeforeExportData(BeforeExportDataEventArgs e)
        {
            _totalRows = _totalRows + e.Rows.Count;
            Console.WriteLine("[{0}] Last read: {1}", DateTime.Now, e.Rows.Count);
        }
        private static void AfterExportData(AfterExportDataEventArgs e)
        {            
            Console.WriteLine("[{0}] Total read: {1}", DateTime.Now, _totalRows);
            Console.SetCursorPosition(0, Console.CursorTop - 2);
        }
    }
}

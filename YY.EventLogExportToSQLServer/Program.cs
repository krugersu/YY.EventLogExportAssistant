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
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Не передан путь к файлам журнала регистрации.");
            }
            else
            {
                string eventLogPath = args[0];

                using (EventLogExportMaster exporter = new EventLogExportMaster())
                {
                    exporter.SetEventLogPath(eventLogPath);
                    exporter.SetWatchPeriod(60);
                    exporter.SetTarget(new EventLogOnSQLServer() { });

                    if (exporter.NewDataAvailiable())
                    {
                        exporter.SendData();
                    }

                    exporter.BeforeExportData += BeforeExportData;
                    exporter.AfterExportData += AfterExportData;

                    exporter.BeginWatch();

                    exporter.EndWatch();

                    Console.WriteLine("Нажмите 'q' для выхода...");
                    while (Console.Read() != 'q') ;
                }
            }     

        }

        private static void AfterExportData(AfterExportDataEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void BeforeExportData(BeforeExportDataEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}

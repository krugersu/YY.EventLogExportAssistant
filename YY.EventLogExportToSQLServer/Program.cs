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
            using (EventLogContext context = new EventLogContext())
            {
                long isCount = context.InformationSystems.Count();
                Console.WriteLine("Количество информационных систем: {0}", isCount);
            }

            if (args.Length == 0)
            {
                Console.WriteLine("Не передан путь к файлам журнала регистрации.");
            }
            else
            {
                string eventLogPath = args[0];
                int queueLength = 1000;
                using (EventLogOnTarget<LogObject> target = new EventLogOnSQLServer<LogObject>())
                {
                    using (EventLogExportMaster<LogObject> exporter = EventLogExportMaster<LogObject>.CreateExportMaster(target))
                    {
                        //using (EventLogReader reader = EventLogReader.CreateReader(eventLogPath))
                        //{
                        //    if (reader.Read())
                        //    {
                        //        exporter.AddItem(reader.CurrentRow);

                        //        // Отправляем порцию накопившихся элементов
                        //        if (exporter.QueueLength >= queueLength)
                        //            exporter.Send();
                        //    }
                        //}

                        // Отправляем оставшиеся элементы
                        if (exporter.QueueLength > 0)
                            exporter.Send();
                    }
                }
            }        

            Console.WriteLine("Для выхода нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}

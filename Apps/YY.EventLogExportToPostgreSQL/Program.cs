using System;
using YY.EventLogReaderAssistant;
using YY.EventLogExportAssistant;
using YY.EventLogExportAssistant.PostgreSQL;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace YY.EventLogExportToPostgreSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (EventLogContext context = new EventLogContext())
            //{
            //    long isCount = context.InformationSystems.Count();
            //    Console.WriteLine("Количество информационных систем: {0}", isCount);
            //}

            //if (args.Length == 0)
            //{
            //    Console.WriteLine("Не передан путь к файлам журнала регистрации.");
            //}
            //else
            //{
            //    string eventLogPath = args[0];
            //    int queueLength = 1000;
            //    EventLogOnTarget exporterDefination = new EventLogOnPostgreSQL();

            //    using (EventLogExportMaster exporter = EventLogExportMaster.CreateExportMaster(exporterDefination))
            //    {
            //        using (EventLogReader reader = EventLogReader.CreateReader(eventLogPath))
            //        {
            //            if (reader.Read())
            //            {
            //                exporter.AddItem(reader.CurrentRow);

            //                // Отправляем порцию накопившихся элементов
            //                if (exporter.QueueLength >= queueLength)
            //                    exporter.Send();
            //            }
            //        }

            //        // Отправляем оставшиеся элементы
            //        if (exporter.QueueLength > 0)
            //            exporter.Send();
            //    }
            //}

            Console.WriteLine("Для выхода нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}

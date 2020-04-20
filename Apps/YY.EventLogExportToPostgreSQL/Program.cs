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
            string connectinString = "";
            DbContextOptionsBuilder<EventLogContext> optionsBuilder = new DbContextOptionsBuilder<EventLogContext>();
            optionsBuilder.UseNpgsql(connectinString);
            
            using (EventLogContext context = new EventLogContext(optionsBuilder.Options))
            {
                long isCount = context.InformationSystems.Count();
                Console.WriteLine(isCount);
            }

            //if (args.Length == 0)
            //{
            //    Console.WriteLine("Не передан путь к файлам журнала регистрации.");
            //    return;
            //}

            //string eventLogPath = args[0];
            //int queueLength = 1000;
            //EventLogTargetDefinition exporterDefination = new EventLogTargetDefinitionForPostgreSQL();

            //using (EventLogExportMaster exporter = EventLogExportMaster.CreateExportMaster(exporterDefination)) 
            //{
            //    using (EventLogReader reader = EventLogReader.CreateReader(eventLogPath))
            //    {
            //        while (reader.Read())
            //        {
            //            exporter.AddItem(reader.CurrentRow);

            //            // Отправляем порцию накопившихся элементов
            //            if(exporter.QueueLength >= queueLength)                        
            //                exporter.Send();                        
            //        }
            //    }

            //    // Отправляем оставшиеся элементы
            //    if (exporter.QueueLength > 0)                
            //        exporter.Send();                
            //}

            Console.WriteLine("Для выхода нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}

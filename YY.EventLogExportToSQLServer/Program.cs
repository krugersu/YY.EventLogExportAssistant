using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using YY.EventLogExportAssistant.SQLServer;

namespace YY.EventLogExportToSQLServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectinString = "";
            DbContextOptionsBuilder<EventLogContext> optionsBuilder = new DbContextOptionsBuilder<EventLogContext>();
            optionsBuilder.UseSqlServer(connectinString);

            using (EventLogContext context = new EventLogContext(optionsBuilder.Options))
            {
                long isCount = context.InformationSystems.Count();
                Console.WriteLine(isCount);
            }

            Console.WriteLine("Hello World!");

            Console.WriteLine("Для выхода нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}

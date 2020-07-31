using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace YY.EventLogExportAssistant.Database
{
    public interface IEventLogContextExtensionActions
    {
        void AdditionalInitializationActions(DatabaseFacade database);
        void OnModelCreating(ModelBuilder modelBuilder, out bool standardBehaviorChanged);
        void OnConfiguring(DbContextOptionsBuilder optionsBuilder);
    }
}

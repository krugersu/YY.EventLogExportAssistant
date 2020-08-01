using System.Collections.Generic;
using System.Linq;
using YY.EventLogExportAssistant.Database.Models;

namespace YY.EventLogExportAssistant.Database
{
    public static class EventLogContextExtensions
    {
        #region Public Methods
        
        public static void FillReferencesToSave(this EventLogContext context, InformationSystemsBase system, ReferencesData data)
        {
            FillReferencesToSave(context, system, data.GetReferencesListForDatabaseType<Applications>(system));
            FillReferencesToSave(context, system, data.GetReferencesListForDatabaseType<Computers>(system));
            FillReferencesToSave(context, system, data.GetReferencesListForDatabaseType<Events>(system));
            FillReferencesToSave(context, system, data.GetReferencesListForDatabaseType<Metadata>(system));
            FillReferencesToSave(context, system, data.GetReferencesListForDatabaseType<PrimaryPorts>(system));
            FillReferencesToSave(context, system, data.GetReferencesListForDatabaseType<SecondaryPorts>(system));
            FillReferencesToSave(context, system, data.GetReferencesListForDatabaseType<Severities>(system));
            FillReferencesToSave(context, system, data.GetReferencesListForDatabaseType<TransactionStatuses>(system));
            FillReferencesToSave(context, system, data.GetReferencesListForDatabaseType<Users>(system));
            FillReferencesToSave(context, system, data.GetReferencesListForDatabaseType<WorkServers>(system));
        }
        public static void AddReferenceToSaveInDB<T>(this EventLogContext context, T item) where T : ReferenceObject
        {
            context.Set<T>().Add(item);
        }

        public static bool ReferenceExistInDB<T>(this EventLogContext context, InformationSystemsBase system, T item) where T : ReferenceObject
        {
            T foundItem = context.Set<T>()
                .FirstOrDefault(e => e.InformationSystemId == system.Id && e.Name == item.Name);

            return (foundItem != null);
        }

        #endregion

        #region Private Methods

        private static void FillReferencesToSave<T>(EventLogContext context, InformationSystemsBase system, IReadOnlyList<T> sourceReferences)
            where T : ReferenceObject
        {
            if (sourceReferences == null)
                return;

            foreach (var itemReference in sourceReferences)
            {
                if (!context.ReferenceExistInDB<T>(system, itemReference))
                {
                    context.AddReferenceToSaveInDB<T>(itemReference);
                }
            }
        }

        #endregion
    }
}

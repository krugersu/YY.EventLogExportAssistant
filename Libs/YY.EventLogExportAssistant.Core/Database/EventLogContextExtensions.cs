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

        #endregion

        #region Private Methods

        private static void FillReferencesToSave<T>(EventLogContext context, InformationSystemsBase system, IReadOnlyList<T> sourceReferences)
            where T : IDatabaseReferenceItem
        {
            if (sourceReferences == null)
                return;

            foreach (var itemReference in sourceReferences)
            {
                if (!itemReference.ReferenceExistInDB(context, system))
                {
                    itemReference.AddReferenceToSaveInDB(context, system);
                }
            }
        }

        #endregion
    }
}

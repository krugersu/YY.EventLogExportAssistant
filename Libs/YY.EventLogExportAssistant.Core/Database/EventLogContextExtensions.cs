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
            FillReferencesToSave(context, system, Applications.PrepearedItemsToSave(system, data));
            FillReferencesToSave(context, system, Computers.PrepearedItemsToSave(system, data));
            FillReferencesToSave(context, system, Events.PrepearedItemsToSave(system, data));
            FillReferencesToSave(context, system, Metadata.PrepearedItemsToSave(system, data));
            FillReferencesToSave(context, system, PrimaryPorts.PrepearedItemsToSave(system, data));
            FillReferencesToSave(context, system, SecondaryPorts.PrepearedItemsToSave(system, data));
            FillReferencesToSave(context, system, Severities.PrepearedItemsToSave(system, data));
            FillReferencesToSave(context, system, TransactionStatuses.PrepearedItemsToSave(system, data));
            FillReferencesToSave(context, system, Users.PrepearedItemsToSave(system, data));
            FillReferencesToSave(context, system, WorkServers.PrepearedItemsToSave(system, data));
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

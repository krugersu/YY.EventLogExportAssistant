using System.Collections.Generic;
using System.Linq;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class WorkServers : ReferenceObject
    {
        #region Public Methods

        public static IReadOnlyList<WorkServers> PrepearedItemsToSave(InformationSystemsBase system, ReferencesData data)
        {
            return data.WorkServers.Select(e =>
                new WorkServers()
                {
                    InformationSystemId = system.Id,
                    Name = e.Name
                }).ToList().AsReadOnly();
        }
        public override bool ReferenceExistInDB(EventLogContext context, InformationSystemsBase system)
        {
            WorkServers foundItem = context.WorkServers
                .FirstOrDefault(e => e.InformationSystemId == InformationSystemId && e.Name == Name);

            if (foundItem == null)
                return false;
            else
                return true;
        }
        public override void AddReferenceToSaveInDB(EventLogContext context, InformationSystemsBase system)
        {
            context.WorkServers.Add(this);
        }

        #endregion
    }
}

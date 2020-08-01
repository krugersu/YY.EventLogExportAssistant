using System.Collections.Generic;
using System.Linq;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class Events : ReferenceObject
    {
        #region Public Methods

        public static IReadOnlyList<Events> PrepearedItemsToSave(InformationSystemsBase system, ReferencesData data)
        {
            return data.Events.Select(e =>
                new Events()
                {
                    InformationSystemId = system.Id,
                    Name = e.Name
                }).ToList().AsReadOnly();
        }
        public override bool ReferenceExistInDB(EventLogContext context, InformationSystemsBase system)
        {
            Events foundItem = context.Events
                .FirstOrDefault(e => e.InformationSystemId == InformationSystemId && e.Name == Name);

            if (foundItem == null)
                return false;
            else
                return true;
        }
        public override void AddReferenceToSaveInDB(EventLogContext context, InformationSystemsBase system)
        {
            context.Events.Add(this);
        }

        #endregion
    }
}

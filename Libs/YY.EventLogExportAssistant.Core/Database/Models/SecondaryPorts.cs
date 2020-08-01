using System.Collections.Generic;
using System.Linq;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class SecondaryPorts : ReferenceObject
    {
        #region Public Methods

        public static IReadOnlyList<SecondaryPorts> PrepearedItemsToSave(InformationSystemsBase system, ReferencesData data)
        {
            return data.SecondaryPorts.Select(e =>
                new SecondaryPorts()
                {
                    InformationSystemId = system.Id,
                    Name = e.Name
                }).ToList().AsReadOnly();
        }
        public override bool ReferenceExistInDB(EventLogContext context, InformationSystemsBase system)
        {
            SecondaryPorts foundItem = context.SecondaryPorts
                .FirstOrDefault(e => e.InformationSystemId == InformationSystemId && e.Name == Name);

            if (foundItem == null)
                return false;
            else
                return true;
        }
        public override void AddReferenceToSaveInDB(EventLogContext context, InformationSystemsBase system)
        {
            context.SecondaryPorts.Add(this);
        }

        #endregion
    }
}

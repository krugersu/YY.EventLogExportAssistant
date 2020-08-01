using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class PrimaryPorts : ReferenceObject
    {
        #region Public Methods

        public static IReadOnlyList<PrimaryPorts> PrepearedItemsToSave(InformationSystemsBase system, ReferencesData data)
        {
            return data.PrimaryPorts.Select(e =>
                new PrimaryPorts()
                {
                    InformationSystemId = system.Id,
                    Name = e.Name
                }).ToList().AsReadOnly();
        }
        public override bool ReferenceExistInDB(EventLogContext context, InformationSystemsBase system)
        {
            PrimaryPorts foundItem = context.PrimaryPorts
                .FirstOrDefault(e => e.InformationSystemId == InformationSystemId && e.Name == Name);

            if (foundItem == null)
                return false;
            else
                return true;
        }
        public override void AddReferenceToSaveInDB(EventLogContext context, InformationSystemsBase system)
        {
            context.PrimaryPorts.Add(this);
        }

        #endregion
    }
}

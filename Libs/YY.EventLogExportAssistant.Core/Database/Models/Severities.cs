using System.Collections.Generic;
using System.Linq;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class Severities : ReferenceObject
    {
        #region Public Methods

        public static IReadOnlyList<Severities> PrepearedItemsToSave(InformationSystemsBase system, ReferencesData data)
        {
            return data.Severities.Select(e =>
                new Severities()
                {
                    InformationSystemId = system.Id,
                    Name = e.ToString()
                }).ToList().AsReadOnly();
        }
        public override bool ReferenceExistInDB(EventLogContext context, InformationSystemsBase system)
        {
            Severities foundItem = context.Severities
                .FirstOrDefault(e => e.InformationSystemId == InformationSystemId && e.Name == Name);

            if (foundItem == null)
                return false;
            else
                return true;
        }
        public override void AddReferenceToSaveInDB(EventLogContext context, InformationSystemsBase system)
        {
            context.Severities.Add(this);
        }

        #endregion
    }
}

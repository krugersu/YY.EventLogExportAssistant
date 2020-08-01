using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class Applications : ReferenceObject
    {
        #region Public Methods
        
        public static IReadOnlyList<Applications> PrepearedItemsToSave(InformationSystemsBase system, ReferencesData data)
        {
            return data.Applications.Select(e => 
            new Applications()
            {
                InformationSystemId = system.Id,
                Name = e.Name
            }).ToList().AsReadOnly();
        }
        public override bool ReferenceExistInDB(EventLogContext context, InformationSystemsBase system)
        {
            Applications foundItem = context.Applications
                .FirstOrDefault(e => e.InformationSystemId == InformationSystemId && e.Name == Name);

            if (foundItem == null)
                return false;
            else
                return true;
        }
        public override void AddReferenceToSaveInDB(EventLogContext context, InformationSystemsBase system)
        {
            context.Applications.Add(this);
        }

        #endregion
    }
}

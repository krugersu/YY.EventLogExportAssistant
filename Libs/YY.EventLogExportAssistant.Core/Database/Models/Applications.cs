using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class Applications : ReferenceObject
    {
        #region Public Methods
        
        public override bool ReferenceExistInDB(EventLogContext context, InformationSystemsBase system)
        {
            Applications foundItem = context.Applications
                .FirstOrDefault(e => e.InformationSystemId == InformationSystemId && e.Name == Name);

            if (foundItem == null)
                return false;
            else
                return true;
        }

        #endregion
    }
}

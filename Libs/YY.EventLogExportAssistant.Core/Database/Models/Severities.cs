using System.Collections.Generic;
using System.Linq;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class Severities : ReferenceObject
    {
        #region Public Methods
        
        public override bool ReferenceExistInDB(EventLogContext context, InformationSystemsBase system)
        {
            Severities foundItem = context.Severities
                .FirstOrDefault(e => e.InformationSystemId == InformationSystemId && e.Name == Name);

            if (foundItem == null)
                return false;
            else
                return true;
        }

        #endregion
    }
}

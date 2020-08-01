using System.Collections.Generic;
using System.Linq;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class WorkServers : ReferenceObject
    {
        #region Public Methods

        public override bool ReferenceExistInDB(EventLogContext context, InformationSystemsBase system)
        {
            WorkServers foundItem = context.WorkServers
                .FirstOrDefault(e => e.InformationSystemId == InformationSystemId && e.Name == Name);

            if (foundItem == null)
                return false;
            else
                return true;
        }

        #endregion
    }
}

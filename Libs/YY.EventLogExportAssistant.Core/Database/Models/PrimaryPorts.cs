using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class PrimaryPorts : ReferenceObject
    {
        #region Public Methods

        public override bool ReferenceExistInDB(EventLogContext context, InformationSystemsBase system)
        {
            PrimaryPorts foundItem = context.PrimaryPorts
                .FirstOrDefault(e => e.InformationSystemId == InformationSystemId && e.Name == Name);

            if (foundItem == null)
                return false;
            else
                return true;
        }

        #endregion
    }
}

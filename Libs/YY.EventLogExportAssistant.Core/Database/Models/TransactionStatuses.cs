using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class TransactionStatuses : ReferenceObject
    {
        #region Public Methods

        public override bool ReferenceExistInDB(EventLogContext context, InformationSystemsBase system)
        {
            TransactionStatuses foundItem = context.TransactionStatuses
                .FirstOrDefault(e => e.InformationSystemId == InformationSystemId && e.Name == Name);

            if (foundItem == null)
                return false;
            else
                return true;
        }

        #endregion
    }
}

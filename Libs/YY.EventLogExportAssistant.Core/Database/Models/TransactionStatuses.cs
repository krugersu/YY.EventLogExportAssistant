using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class TransactionStatuses : ReferenceObject
    {
        #region Public Methods

        public static IReadOnlyList<TransactionStatuses> PrepearedItemsToSave(InformationSystemsBase system, ReferencesData data)
        {
            return data.TransactionStatuses.Select(e =>
                new TransactionStatuses()
                {
                    InformationSystemId = system.Id,
                    Name = e.ToString()
                }).ToList().AsReadOnly();
        }
        public override bool ReferenceExistInDB(EventLogContext context, InformationSystemsBase system)
        {
            TransactionStatuses foundItem = context.TransactionStatuses
                .FirstOrDefault(e => e.InformationSystemId == InformationSystemId && e.Name == Name);

            if (foundItem == null)
                return false;
            else
                return true;
        }
        public override void AddReferenceToSaveInDB(EventLogContext context, InformationSystemsBase system)
        {
            context.TransactionStatuses.Add(this);
        }

        #endregion
    }
}

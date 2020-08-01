using System;
using System.Collections.Generic;
using System.Linq;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class Users : ReferenceObject
    {
        #region Public Properties

        public Guid Uuid { get; set; }

        #endregion

        #region Public Methods

        public static IReadOnlyList<Users> PrepearedItemsToSave(InformationSystemsBase system, ReferencesData data)
        {
            return data.Users.Select(e =>
                new Users()
                {
                    InformationSystemId = system.Id,
                    Name = e.Name,
                    Uuid = e.Uuid
                }).ToList().AsReadOnly();
        }
        public override bool ReferenceExistInDB(EventLogContext context, InformationSystemsBase system)
        {
            Users foundItem = context.Users
                .FirstOrDefault(e => e.InformationSystemId == InformationSystemId && e.Name == Name);

            if (foundItem == null)
                return false;
            else
                return true;
        }
        public override void AddReferenceToSaveInDB(EventLogContext context, InformationSystemsBase system)
        {
            context.Users.Add(this);
        }

        #endregion
    }
}

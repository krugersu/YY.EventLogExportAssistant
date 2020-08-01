using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace YY.EventLogExportAssistant.Database.Models
{
    public abstract class ReferenceObject : CommonLogObject, IDatabaseReferenceItem
    {
        #region Public Static Methods

        public static IReadOnlyList<T> PrepareItemsToSave<T>(InformationSystemsBase system, ReferencesData data) where T : IDatabaseReferenceItem
        {
            IReadOnlyList<T> sourceReferenceList = data.GetReferencesListForDatabaseType<T>(system);

            return sourceReferenceList;
        }

        #endregion

        #region Public Properties

        [MaxLength(250)]
        public string Name { get; set; }

        #endregion

        #region Public Methods
        
        public virtual void AddReferenceToSaveInDB(EventLogContext context, InformationSystemsBase system)
        {
            throw new System.NotImplementedException();
        }
        public virtual bool ReferenceExistInDB(EventLogContext context, InformationSystemsBase system)
        {
            throw new System.NotImplementedException();
        }
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}

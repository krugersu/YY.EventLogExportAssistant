using System.ComponentModel.DataAnnotations;

namespace YY.EventLogExportAssistant.Database.Models
{
    public abstract class ReferenceObject : CommonLogObject, IDatabaseReferenceItem
    {
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

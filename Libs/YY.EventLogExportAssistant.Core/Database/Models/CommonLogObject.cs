using System.ComponentModel.DataAnnotations;

namespace YY.EventLogExportAssistant.Database.Models
{
    public abstract class CommonLogObject
    {
        #region Public Properties

        public long InformationSystemId { get; set; }
        public long Id { get; set; }

        #endregion
    }
}

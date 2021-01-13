using System.ComponentModel.DataAnnotations;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class InformationSystems : InformationSystemsBase
    {
        #region Public Properties

        public override long Id { get; set; }
        [MaxLength(250)]
        public override string Name { get; set; }
        [MaxLength(500)]
        public override string Description { get; set; }
        [MaxLength(500)] 
        public override string TimeZoneName { get; set; }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}

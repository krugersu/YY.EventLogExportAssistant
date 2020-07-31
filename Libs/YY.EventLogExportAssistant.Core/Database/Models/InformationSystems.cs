using System.ComponentModel.DataAnnotations;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class InformationSystems : InformationSystemsBase
    {
        public override long Id { get; set; }
        [MaxLength(250)]
        public override string Name { get; set; }
        [MaxLength(500)]
        public override string Description { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}

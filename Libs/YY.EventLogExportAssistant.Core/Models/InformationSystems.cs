using System.ComponentModel.DataAnnotations;

namespace YY.EventLogExportAssistant
{
    public class InformationSystems
    {
        public long Id { get; set; }
        [MaxLength(250)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}

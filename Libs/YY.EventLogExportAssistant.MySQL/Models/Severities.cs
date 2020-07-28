using System.ComponentModel.DataAnnotations;

namespace YY.EventLogExportAssistant.MySQL.Models
{
    public class Severities : CommonLogObject
    {
        public long Id { get; set; }
        [MaxLength(250)]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}

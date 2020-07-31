using System.ComponentModel.DataAnnotations;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class Events : CommonLogObject
    {
        [MaxLength(250)]
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}

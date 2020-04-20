using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YY.EventLogExportAssistant.PostgreSQL.Models
{
    public class Applications : LogObject
    {
        public long id { get; set; }
        [MaxLength(250)]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}

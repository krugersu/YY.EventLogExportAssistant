using System.ComponentModel.DataAnnotations;

namespace YY.EventLogExportAssistant.SQLServer.Models
{
    public class WorkServers : CommonLogObject
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

using System;
using System.ComponentModel.DataAnnotations;

namespace YY.EventLogExportAssistant.MySQL.Models
{
    public class Users : CommonLogObject
    {
        public long Id { get; set; }
        public Guid Uuid { get; set; }
        [MaxLength(250)]
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}

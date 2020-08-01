using System;
using System.Collections.Generic;
using System.Linq;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class Users : ReferenceObject
    {
        #region Public Properties

        public Guid Uuid { get; set; }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace YY.EventLogExportAssistant.Database
{
    public interface IDatabaseReferenceItem
    {
        bool ReferenceExistInDB(EventLogContext context, InformationSystemsBase system);
    }
}

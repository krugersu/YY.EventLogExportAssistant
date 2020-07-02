using System;
using System.Collections.Generic;
using System.Text;

namespace YY.EventLogExportAssistant.ElasticSearch
{
    public enum IndexSeparationPeriod
    {
        None,
        Hour,
        Day,
        Week,
        Month,
        Quarter,
        HalfYear,
        Year
    }
}

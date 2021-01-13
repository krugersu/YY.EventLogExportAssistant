using System;

namespace YY.EventLogExportAssistant
{
    public class InformationSystemsBase
    {
        #region Public Members

        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string TimeZoneName { get; set; }

        #endregion

        #region Constructors

        public InformationSystemsBase()
        {
            TimeZoneName = TimeZoneInfo.Local.Id;
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}

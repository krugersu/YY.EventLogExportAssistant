using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace YY.EventLogExportAssistant.Database.Models
{
    public class InformationSystems : InformationSystemsBase
    {
        #region Public Properties

        public override long Id { get; set; }
        [MaxLength(250)]
        public override string Name { get; set; }
        [MaxLength(500)]
        public override string Description { get; set; }
        [MaxLength(500)]
        public override string TimeZoneName
        {
            get => _timeZoneName;
            set
            {
                _timeZoneName = value;
                _timeZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(t => t.Id == _timeZoneName);
                _timeZoneRecognized = (_timeZone != null);
            }
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

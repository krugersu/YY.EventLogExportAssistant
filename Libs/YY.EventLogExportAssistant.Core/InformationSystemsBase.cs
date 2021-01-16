using System;
using System.Linq;

namespace YY.EventLogExportAssistant
{
    public class InformationSystemsBase
    {
        #region Private Members

        protected string _timeZoneName;
        protected bool _timeZoneRecognized;
        protected TimeZoneInfo _timeZone;

        #endregion

        #region Public Members

        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public virtual string TimeZoneName
        {
            get => _timeZoneName;
            set
            {
                _timeZoneName = value;
                if (string.IsNullOrEmpty(_timeZoneName))
                {
                    _timeZone = TimeZoneInfo.Local;
                    _timeZoneName = _timeZone.Id;
                }
                else
                {
                    _timeZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(t => t.Id == _timeZoneName);
                }
                _timeZoneRecognized = (_timeZone != null);
            }
        }

        public virtual TimeZoneInfo TimeZone
        {
            get
            {
                if (_timeZoneRecognized)
                {
                    return _timeZone;
                }

                throw new TimeZoneNotFoundException();
            }
        }

        #endregion

        #region Constructors

        public InformationSystemsBase()
        {
            _timeZone = TimeZoneInfo.Local;
            _timeZoneName = _timeZone.Id;
            _timeZoneRecognized = true;
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

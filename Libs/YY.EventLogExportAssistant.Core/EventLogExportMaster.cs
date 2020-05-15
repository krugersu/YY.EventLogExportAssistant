using System.Collections.Generic;
using System.IO;
using YY.EventLogReaderAssistant;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportAssistant
{
    public sealed class EventLogExportMaster : IEventLogExportMaster
    {
        #region Private Member Variables

        private string _eventLogPath;
        private string _referenceDataHash;
        private IEventLogOnTarget _target;

        public delegate void BeforeExportDataHandler(BeforeExportDataEventArgs e);
        public event BeforeExportDataHandler BeforeExportData;

        public delegate void AfterExportDataHandler(AfterExportDataEventArgs e);
        public event AfterExportDataHandler AfterExportData;

        #endregion

        #region Constructor

        public EventLogExportMaster()
        {
            _referenceDataHash = string.Empty;
        }

        #endregion

        #region Public Methods

        public void SetEventLogPath(string eventLogPath)
        {
            _eventLogPath = eventLogPath;
        }
        public void SetTarget(IEventLogOnTarget target)
        {
            _target = target;
        }
        public bool NewDataAvailiable()
        {
            EventLogPosition lastPosition = _target.GetLastPosition();

            bool newDataExist = false;
            using (EventLogReader reader = EventLogReader.CreateReader(_eventLogPath))
            {
                reader.SetCurrentPosition(lastPosition);
                newDataExist = reader.Read();
            }

            return newDataExist;
        }
        public void SendData()
        {
            EventLogPosition lastPosition = _target.GetLastPosition();
            using (EventLogReader reader = EventLogReader.CreateReader(_eventLogPath))
            {
                reader.AfterReadFile += EventLogReader_AfterReadFile;
                reader.SetCurrentPosition(lastPosition);

                int portionSize = _target.GetPortionSize();
                List<RowData> dataToSend = new List<RowData>();

                while (reader.Read())
                {
                    if (reader.CurrentRow == null)
                        continue;

                    dataToSend.Add(reader.CurrentRow);

                    if (dataToSend.Count >= portionSize)
                    {
                        bool cancel = false;
                        if (BeforeExportData != null)
                        {
                            BeforeExportDataEventArgs beforeExportArgs = new BeforeExportDataEventArgs()
                            {
                                Rows = dataToSend
                            };
                            BeforeExportData.Invoke(beforeExportArgs);
                            cancel = beforeExportArgs.Cancel;
                        }

                        EventLogPosition currentPosition = reader.GetCurrentPosition();
                        if (!cancel)
                        {
                            UpdateReferences(reader);
                            _target.Save(dataToSend);

                            AfterExportData.Invoke(new AfterExportDataEventArgs()
                            {
                                CurrentPosition = currentPosition
                            });
                        }

                        if (reader.CurrentFile != null)
                        {
                            FileInfo logFileInfo = new FileInfo(reader.CurrentFile);
                            _target.SaveLogPosition(logFileInfo, currentPosition);
                        }

                        dataToSend.Clear();
                        break;
                    }
                }

                if (dataToSend.Count > 0)
                {
                    bool cancel = false;
                    if (BeforeExportData != null)
                    {
                        BeforeExportDataEventArgs beforeExportArgs = new BeforeExportDataEventArgs()
                        {
                            Rows = dataToSend
                        };
                        BeforeExportData.Invoke(beforeExportArgs);
                        cancel = beforeExportArgs.Cancel;
                    }

                    EventLogPosition currentPosition = reader.GetCurrentPosition();
                    if (!cancel)
                    {
                        UpdateReferences(reader);
                        _target.Save(dataToSend);

                        AfterExportData.Invoke(new AfterExportDataEventArgs()
                        {
                            CurrentPosition = currentPosition
                        });
                    }

                    if (reader.CurrentFile != null)
                    {
                        FileInfo logFileInfo = new FileInfo(reader.CurrentFile);
                        _target.SaveLogPosition(logFileInfo, currentPosition);
                    }

                    dataToSend.Clear();
                }
            }
        }

        #endregion

        #region Private Methods

        private void EventLogReader_AfterReadFile(EventLogReader sender, AfterReadFileEventArgs args)
        {
            FileInfo _lastEventLogDataFileInfo = new FileInfo(args.FileName);
            EventLogPosition position = sender.GetCurrentPosition();
            _target.SaveLogPosition(_lastEventLogDataFileInfo, position);
        }
        private void UpdateReferences(EventLogReader reader)
        {
            if (_referenceDataHash != reader.ReferencesHash)
            {
                List<Severity> severities = new List<Severity>
                {
                    Severity.Error,
                    Severity.Information,
                    Severity.Note,
                    Severity.Unknown,
                    Severity.Warning
                };

                List<TransactionStatus> transactionStatuses = new List<TransactionStatus>
                {
                    TransactionStatus.Committed,
                    TransactionStatus.NotApplicable,
                    TransactionStatus.RolledBack,
                    TransactionStatus.Unfinished,
                    TransactionStatus.Unknown
                };

                ReferencesData data = new ReferencesData()
                {
                    Applications = reader.Applications,
                    Computers = reader.Computers,
                    Events = reader.Events,
                    Metadata = reader.Metadata,
                    PrimaryPorts = reader.PrimaryPorts,
                    SecondaryPorts = reader.SecondaryPorts,
                    Severities = severities.AsReadOnly(),
                    TransactionStatuses = transactionStatuses.AsReadOnly(),
                    Users = reader.Users,
                    WorkServers = reader.WorkServers
                };
                _target.UpdateReferences(data);
                _referenceDataHash = reader.ReferencesHash;
            }
        }

        #endregion
    }
}

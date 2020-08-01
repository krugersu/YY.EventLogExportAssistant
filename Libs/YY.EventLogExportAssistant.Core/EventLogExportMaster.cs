using System.Collections.Generic;
using System.IO;
using YY.EventLogReaderAssistant;
using YY.EventLogReaderAssistant.EventArguments;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportAssistant
{
    public sealed class EventLogExportMaster : IEventLogExportMaster
    {
        #region Private Member Variables

        private string _eventLogPath;
        private string _referenceDataHash;
        private IEventLogOnTarget _target;
        private List<RowData> _dataToSend;
        private int _portionSize;

        public delegate void BeforeExportDataHandler(BeforeExportDataEventArgs e);
        public event BeforeExportDataHandler BeforeExportData;
        public delegate void AfterExportDataHandler(AfterExportDataEventArgs e);
        public event AfterExportDataHandler AfterExportData;
        public delegate void OnErrorExportDataHandler(OnErrorExportDataEventArgs e);
        public event OnErrorExportDataHandler OnErrorExportData;

        #endregion

        #region Constructor

        public EventLogExportMaster()
        {
            _referenceDataHash = string.Empty;
            _dataToSend = new List<RowData>();
            _portionSize = 0;
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
            if (_target != null)
            {
                _portionSize = _target.GetPortionSize();
            }
        }
        public bool NewDataAvailiable()
        {
            if (_target == null)
                return false;
            if (_eventLogPath == null)
                return false;

            EventLogPosition lastPosition = _target.GetLastPosition();

            bool newDataExist;
            using (EventLogReader reader = EventLogReader.CreateReader(_eventLogPath))
            {
                // В случае, если каталог последней позиции не совпадает 
                // с текущим каталогом данных, то предыдущую позицию не учитываем
                if (lastPosition != null)
                {
                    FileInfo lastDataFileInfo = new FileInfo(lastPosition.CurrentFileReferences);
                    FileInfo currentDataFileInfo = new FileInfo(reader.CurrentFile);

                    if (lastDataFileInfo.Directory != null && currentDataFileInfo.Directory != null)
                    {
                        if (lastDataFileInfo.Directory.FullName != currentDataFileInfo.Directory.FullName)
                            lastPosition = null;
                    }
                }
                reader.SetCurrentPosition(lastPosition);
                newDataExist = reader.Read();
            }

            return newDataExist;
        }
        public void SendData()
        {
            if (_target == null)
                return;
            if (_eventLogPath == null)
                return;

            EventLogPosition lastPosition = _target.GetLastPosition();
            using (EventLogReader reader = EventLogReader.CreateReader(_eventLogPath))
            {
                reader.AfterReadFile += EventLogReader_AfterReadFile;
                reader.AfterReadEvent += EventLogReader_AfterReadEvent;
                reader.OnErrorEvent += EventLogReader_OnErrorEvent;
                reader.SetCurrentPosition(lastPosition);

                long totalReadEvents = 0;
                while (reader.Read())
                {
                    if(reader.CurrentRow != null)
                        totalReadEvents += 1;

                    if (totalReadEvents >= _portionSize)
                        break;
                }

                if (_dataToSend.Count > 0)                
                    SendDataCurrentPortion(reader);                
            }
        }

        #endregion

        #region Private Methods

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
        private void SendDataCurrentPortion(EventLogReader reader)
        {
            RiseBeforeExportData(out var cancel);
            if (!cancel)
            {
                UpdateReferences(reader);
                _target.Save(_dataToSend);
                RiseAfterExportData(reader.GetCurrentPosition());
            }

            if (reader.CurrentFile != null)
            {
                _target.SaveLogPosition(
                    new FileInfo(reader.CurrentFile),
                    reader.GetCurrentPosition());
            }
            _dataToSend.Clear();
        }

        private void RiseAfterExportData(EventLogPosition currentPosition)
        {
            AfterExportDataHandler handlerAfterExportData = AfterExportData;
            handlerAfterExportData?.Invoke(new AfterExportDataEventArgs()
            {
                CurrentPosition = currentPosition
            });

        }
        private void RiseBeforeExportData(out bool cancel)
        {
            BeforeExportDataHandler handlerBeforeExportData = BeforeExportData;
            if (handlerBeforeExportData != null)
            {
                BeforeExportDataEventArgs beforeExportArgs = new BeforeExportDataEventArgs()
                {
                    Rows = _dataToSend
                };
                handlerBeforeExportData.Invoke(beforeExportArgs);
                cancel = beforeExportArgs.Cancel;
            }
            else
            {
                cancel = false;
            }
        }

        #endregion

        #region Events

        private void EventLogReader_AfterReadEvent(EventLogReader sender, AfterReadEventArgs args)
        {
            if (sender.CurrentRow == null)
                return;

            _dataToSend.Add(sender.CurrentRow);

            if (_dataToSend.Count >= _portionSize)
            {
                SendDataCurrentPortion(sender);
            }
        }
        private void EventLogReader_AfterReadFile(EventLogReader sender, AfterReadFileEventArgs args)
        {
            FileInfo _lastEventLogDataFileInfo = new FileInfo(args.FileName);
            EventLogPosition position = sender.GetCurrentPosition();
            _target.SaveLogPosition(_lastEventLogDataFileInfo, position);
        }
        private void EventLogReader_OnErrorEvent(EventLogReader sender, OnErrorEventArgs args)
        {
            OnErrorExportDataHandler handlerOnErrorExportData = OnErrorExportData;
            handlerOnErrorExportData?.Invoke(new OnErrorExportDataEventArgs()
            {
                Exception = args.Exception,
                SourceData = args.SourceData,
                Critical = args.Critical
            });
        }

        #endregion
    }
}

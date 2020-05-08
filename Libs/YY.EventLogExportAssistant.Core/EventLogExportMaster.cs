using System;
using System.Collections.Generic;
using System.IO;
using YY.EventLogReaderAssistant;
using YY.EventLogReaderAssistant.Models;
using System.Timers;

namespace YY.EventLogExportAssistant
{
    public sealed class EventLogExportMaster : IEventLogExportMaster, IDisposable
    {        
        private string _eventLogPath;
        private EventLogReader _eventLogReader;
        private DateTime _lastUpdateReferences;
        private IEventLogOnTarget _target;

        public delegate void BeforeExportDataHandler(BeforeExportDataEventArgs e);
        public event BeforeExportDataHandler BeforeExportData;

        public delegate void AfterExportDataHandler(AfterExportDataEventArgs e);
        public event AfterExportDataHandler AfterExportData;

        public EventLogExportMaster()
        {
            _lastUpdateReferences = DateTime.MinValue;
        }

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
            EventLogReader reader = GetReader();
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

                    EventLogPosition currentPosition = _eventLogReader.GetCurrentPosition();
                    if (!cancel)
                    {
                        UpdateReferences();
                        _target.Save(dataToSend);
                                              
                        AfterExportData.Invoke(new AfterExportDataEventArgs() 
                        { 
                            CurrentPosition = currentPosition
                        });
                    }

                    if (_eventLogReader.CurrentFile != null)
                    {
                        FileInfo logFileInfo = new FileInfo(_eventLogReader.CurrentFile);
                        _target.SaveLogPosition(logFileInfo, currentPosition);
                    }

                    dataToSend.Clear();
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

                EventLogPosition currentPosition = _eventLogReader.GetCurrentPosition();
                if (!cancel)
                {
                    UpdateReferences();
                    _target.Save(dataToSend);
                                     
                    AfterExportData.Invoke(new AfterExportDataEventArgs()
                    {
                        CurrentPosition = currentPosition
                    });                    
                }

                if (_eventLogReader.CurrentFile != null)
                {
                    FileInfo logFileInfo = new FileInfo(_eventLogReader.CurrentFile);
                    _target.SaveLogPosition(logFileInfo, currentPosition);
                }

                dataToSend.Clear();
            }
        }
        public void Dispose()
        {
            if(_eventLogReader != null)
                _eventLogReader.Dispose();
        }

        private EventLogReader GetReader(bool recreate = false)
        {
            if (_eventLogReader == null || recreate)
            {
                if(_eventLogReader != null)
                {
                    _eventLogReader.Dispose();
                    _eventLogReader = null;
                }

                _eventLogReader = EventLogReader.CreateReader(_eventLogPath);
                _eventLogReader.AfterReadFile += EventLogReader_AfterReadFile;
            }

            return _eventLogReader;
        }
        private void EventLogReader_AfterReadFile(EventLogReader sender, AfterReadFileEventArgs args)
        {
            FileInfo _lastEventLogDataFileInfo = new FileInfo(args.FileName);
            EventLogPosition position = sender.GetCurrentPosition();
            _target.SaveLogPosition(_lastEventLogDataFileInfo, position);
        }
        private void UpdateReferences()
        {
            if (_lastUpdateReferences != _eventLogReader.ReferencesReadDate)
            {
                List<Severity> severities = new List<Severity>();
                severities.Add(Severity.Error);
                severities.Add(Severity.Information);
                severities.Add(Severity.Note);
                severities.Add(Severity.Unknown);
                severities.Add(Severity.Warning);

                List<TransactionStatus> transactionStatuses = new List<TransactionStatus>();
                transactionStatuses.Add(TransactionStatus.Committed);
                transactionStatuses.Add(TransactionStatus.NotApplicable);
                transactionStatuses.Add(TransactionStatus.RolledBack);
                transactionStatuses.Add(TransactionStatus.Unfinished);
                transactionStatuses.Add(TransactionStatus.Unknown);

                ReferencesData data = new ReferencesData()
                {
                    Applications = _eventLogReader.Applications,
                    Computers = _eventLogReader.Computers,
                    Events = _eventLogReader.Events,
                    Metadata = _eventLogReader.Metadata,
                    PrimaryPorts = _eventLogReader.PrimaryPorts,
                    SecondaryPorts = _eventLogReader.SecondaryPorts,
                    Severities = severities.AsReadOnly(),
                    TransactionStatuses = transactionStatuses.AsReadOnly(),
                    Users = _eventLogReader.Users,
                    WorkServers = _eventLogReader.WorkServers
                };
                _target.UpdateReferences(data);
                _lastUpdateReferences = _eventLogReader.ReferencesReadDate;
            }
        }
    }
}

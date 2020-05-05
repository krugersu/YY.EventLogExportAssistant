using System;
using System.Collections.Generic;
using System.IO;
using YY.EventLogReaderAssistant;
using YY.EventLogReaderAssistant.Models;

namespace YY.EventLogExportAssistant
{
    public sealed class EventLogExportMaster : IEventLogExportMaster, IDisposable
    {        
        private string _eventLogPath;
        private EventLogReader _eventLogReader;
        private DateTime _lastUpdateReferences;
        private IEventLogOnTarget _target;
        private int _watchPeriod;
        FileSystemWatcher _watcher;

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
        public void SetWatchPeriod(int seconds)
        {
            _watchPeriod = seconds;
        }
        public void SetTarget(IEventLogOnTarget target)
        {
            _target = target;
        }
        public void BeginWatch()
        {
            string dirPath = Path.GetDirectoryName(_eventLogPath);
            _watcher = new FileSystemWatcher(dirPath);
            //_watcher.EnableRaisingEvents = true;

        }
        public void EndWatch()
        {
            throw new NotImplementedException();
        }
        public bool NewDataAvailiable()
        {
            // Получаем последнюю позицию считанных данных
            EventLogPosition lastPosition = _target.GetLastPosition();

            // Проверяем есть ли еще информация после установленной позиции
            bool newDataExist = false;
            EventLogReader reader = GetReader();
            reader.SetCurrentPosition(lastPosition);
            newDataExist = reader.Read();
            reader.Reset();

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

                    if (!cancel)
                    {
                        UpdateReferences();
                        _target.Save(dataToSend);
                        AfterExportData.Invoke(new AfterExportDataEventArgs() 
                        { 
                            CurrentPosition = _eventLogReader.GetCurrentPosition()
                        });
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

                if (!cancel)
                {
                    UpdateReferences();
                    _target.Save(dataToSend);
                    AfterExportData.Invoke(new AfterExportDataEventArgs() { });
                }

                dataToSend.Clear();
            }
        }
        public void Dispose()
        {
            _eventLogReader.Dispose();
        }

        private EventLogReader GetReader()
        {
            if (_eventLogReader == null)
            {
                _eventLogReader = EventLogReader.CreateReader(_eventLogPath);
            }

            return _eventLogReader;
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

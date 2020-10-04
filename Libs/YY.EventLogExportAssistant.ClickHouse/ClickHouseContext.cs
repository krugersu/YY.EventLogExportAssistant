using ClickHouse.Client.ADO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ClickHouse.Client.ADO.Parameters;
using ClickHouse.Client.Copy;
using YY.EventLogExportAssistant.ClickHouse.Helpers;
using YY.EventLogExportAssistant.Database.Models;
using YY.EventLogExportAssistant.Helpers;
using YY.EventLogReaderAssistant;
using RowData = YY.EventLogReaderAssistant.Models.RowData;

namespace YY.EventLogExportAssistant.ClickHouse
{
    public class ClickHouseContext : IDisposable
    {
        #region Private Static Members

        private static readonly string _emptyGuidAsString = Guid.Empty.ToString();

        #endregion

        #region Private Members

        private string _databaseName;
        private ClickHouseConnection _connection;
        private long logFileLastId = -1;

        #endregion

        #region Constructors

        public ClickHouseContext(string connectionSettings)
        {
            CheckDatabaseSettings(connectionSettings);

            _connection = new ClickHouseConnection(connectionSettings);
            _connection.Open();
            
            var cmdDDL = _connection.CreateCommand();

            cmdDDL.CommandText = Resources.Query_CreateTable_RowsData;
            cmdDDL.ExecuteNonQuery();

            cmdDDL.CommandText = Resources.Query_CreateTable_LogFiles;
            cmdDDL.ExecuteNonQuery();
        }

        #endregion

        #region Public Methods

        #region RowsData

        public void SaveRowsData(InformationSystemsBase system, List<RowData> rowsData)
        {
            using (ClickHouseBulkCopy bulkCopyInterface = new ClickHouseBulkCopy(_connection)
            {
                DestinationTableName = "RowsData",
                BatchSize = 100000
            })
            {
                var values = rowsData.Select(i => new object[]
                {
                    system.Name,
                    i.RowId,
                    i.Period.DateTime,
                    Severities.GetPresentationByName(i.Severity.ToString()),
                    i.ConnectId ?? 0,
                    i.Session ?? 0,
                    TransactionStatuses.GetPresentationByName(i.TransactionStatus.ToString()),
                    i.TransactionDate ?? DateTime.MinValue,
                    i.TransactionId ?? 0,
                    i.User?.Name ?? string.Empty,
                    i.User?.Uuid.ToString() ?? _emptyGuidAsString,
                    i.Computer?.Name ?? string.Empty,
                    Applications.GetPresentationByName(i.Application?.Name ?? string.Empty),
                    Events.GetPresentationByName(i.Event?.Name ?? string.Empty),
                    i.Comment ?? string.Empty,
                    i.Metadata?.Name ?? string.Empty,
                    i.Metadata?.Uuid.ToString() ?? _emptyGuidAsString,
                    i.Data ?? string.Empty,
                    (i.DataUuid ?? string.Empty).NormalizeShortUUID(),
                    i.DataPresentation ?? string.Empty,
                    i.WorkServer?.Name ?? string.Empty,
                    i.PrimaryPort?.Name ?? string.Empty,
                    i.SecondaryPort?.Name ?? string.Empty
                }).AsEnumerable();

                var bulkResult = bulkCopyInterface.WriteToServerAsync(values);
                bulkResult.Wait();
            }
        }
        public DateTime GetRowsDataMaxPeriod(InformationSystemsBase system)
        {
            DateTime output = DateTime.MinValue;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        MAX(Period) AS MaxPeriod
                    FROM RowsData AS RD
                    WHERE InformationSystem = {InformationSystem:String} ";
                command.Parameters.Add(new ClickHouseDbParameter
                {
                    ParameterName = "InformationSystem",
                    Value = system.Name
                });
                using (var cmdReader = command.ExecuteReader())
                {
                    if (cmdReader.Read())
                        output = cmdReader.GetDateTime(0);
                }
            }

            return output;
        }
        public bool RowDataExistOnDatabase(InformationSystemsBase system, RowData rowData)
        {
            bool output = false;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        InformationSystem,
                        Id,
                        Period
                    FROM RowsData AS RD
                    WHERE InformationSystem = {existInfSysId:String}
                        AND Id = {existId:Int64}
                        AND Period = {existPeriod:DateTime}";
                command.Parameters.Add(new ClickHouseDbParameter
                {
                    ParameterName = "existInfSysId",
                    DbType = DbType.AnsiString,
                    Value = system.Name
                });
                command.Parameters.Add(new ClickHouseDbParameter
                {
                    ParameterName = "existId",
                    DbType = DbType.Int64,
                    Value = rowData.RowId
                });
                command.Parameters.Add(new ClickHouseDbParameter
                {
                    ParameterName = "existPeriod",
                    DbType = DbType.DateTime,
                    Value = rowData.Period.DateTime
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    if (cmdReader.Read())
                        output = true;
                }
            }

            return output;
        }

        #endregion

        #region LogFiles

        public EventLogPosition GetLogFilePosition(InformationSystemsBase system)
        {
            var cmdGetLastLogFileInfo = _connection.CreateCommand();
            cmdGetLastLogFileInfo.CommandText =
                @"SELECT	                
	                LastEventNumber,
	                LastCurrentFileReferences,
	                LastCurrentFileData,
	                LastStreamPosition
                FROM LogFiles AS LF
                WHERE InformationSystem = {informationSystem:String}
                    AND Id IN (
                        SELECT
                            MAX(Id) LastId
                        FROM LogFiles AS LF_LAST
                        WHERE LF_LAST.InformationSystem = {informationSystem:String}
                    )";
            cmdGetLastLogFileInfo.Parameters.Add(new ClickHouseDbParameter
            {
                ParameterName = "informationSystem",
                DbType = DbType.AnsiString,
                Value = system.Name
            });

            EventLogPosition output = null;
            using (var cmdReader = cmdGetLastLogFileInfo.ExecuteReader())
            {
                if (cmdReader.Read())
                {
                    string fileData = cmdReader.GetString(2)
                        .Replace("\\\\","\\")
                        .FixNetworkPath();
                    string fileReferences = cmdReader.GetString(1)
                        .Replace("\\\\", "\\")
                        .FixNetworkPath();

                    output = new EventLogPosition(
                        cmdReader.GetInt64(0),
                        fileReferences,
                        fileData,
                        cmdReader.GetInt64(3));
                }
            }

            return output;
        }
        public void SaveLogPosition(InformationSystemsBase system, FileInfo logFileInfo, EventLogPosition position)
        {
            var commandAddLogInfo = _connection.CreateCommand();
            commandAddLogInfo.CommandText =
                @"INSERT INTO LogFiles (
                    InformationSystem,
                    Id,
                    FileName,
                    CreateDate,
                    ModificationDate,
                    LastEventNumber,
                    LastCurrentFileReferences,
                    LastCurrentFileData,
                    LastStreamPosition
                ) VALUES (
                    {isId:String},
                    {newId:Int64},
                    {FileName:String},
                    {CreateDate:DateTime},
                    {ModificationDate:DateTime},
                    {LastEventNumber:Int64},
                    {LastCurrentFileReferences:String},
                    {LastCurrentFileData:String},
                    {LastStreamPosition:Int64}     
                )";

            commandAddLogInfo.Parameters.Add(new ClickHouseDbParameter
            {
                ParameterName = "isId",
                DbType = DbType.Int64,
                Value = system.Name
            });
            commandAddLogInfo.Parameters.Add(new ClickHouseDbParameter
            {
                ParameterName = "newId",
                DbType = DbType.Int64,
                Value = GetLogFileInfoNewId(system)
            });
            commandAddLogInfo.Parameters.Add(new ClickHouseDbParameter
            {
                ParameterName = "FileName",
                DbType = DbType.AnsiString,
                Value = logFileInfo.Name
            });
            commandAddLogInfo.Parameters.Add(new ClickHouseDbParameter
            {
                ParameterName = "CreateDate",
                DbType = DbType.DateTime,
                Value = logFileInfo.CreationTimeUtc
            });
            commandAddLogInfo.Parameters.Add(new ClickHouseDbParameter
            {
                ParameterName = "ModificationDate",
                DbType = DbType.DateTime,
                Value = logFileInfo.LastWriteTimeUtc
            });
            commandAddLogInfo.Parameters.Add(new ClickHouseDbParameter
            {
                ParameterName = "LastEventNumber",
                DbType = DbType.Int64,
                Value = position.EventNumber
            });
            commandAddLogInfo.Parameters.Add(new ClickHouseDbParameter
            {
                ParameterName = "LastCurrentFileReferences",
                DbType = DbType.AnsiString,
                Value = position.CurrentFileReferences.Replace("\\", "\\\\")
            });
            commandAddLogInfo.Parameters.Add(new ClickHouseDbParameter
            {
                ParameterName = "LastCurrentFileData",
                DbType = DbType.AnsiString,
                Value = position.CurrentFileData.Replace("\\", "\\\\")
            });
            commandAddLogInfo.Parameters.Add(new ClickHouseDbParameter
            {
                ParameterName = "LastStreamPosition",
                DbType = DbType.Int64,
                Value = position?.StreamPosition ?? 0
            });

            commandAddLogInfo.ExecuteNonQuery();
        }
        public long GetLogFileInfoNewId(InformationSystemsBase system)
        {
            long output = 0;

            if (logFileLastId < 0)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText =
                        @"SELECT
                        MAX(Id)
                    FROM LogFiles
                    WHERE InformationSystem = {InformationSystem:String}";
                    command.Parameters.Add(new ClickHouseDbParameter
                    {
                        ParameterName = "InformationSystem",
                        Value = system.Name
                    });
                    using (var cmdReader = command.ExecuteReader())
                    {
                        if (cmdReader.Read())
                            output = cmdReader.GetInt64(0);
                    }
                }
            }
            else
            {
                output = logFileLastId;
            }

            output += 1;
            logFileLastId = output;

            return output;
        }
        public void RemoveArchiveLogFileRecords(InformationSystemsBase system)
        {
            var commandRemoveArchiveLogInfo = _connection.CreateCommand();
            commandRemoveArchiveLogInfo.CommandText =
                @"ALTER TABLE LogFiles DELETE
                WHERE InformationSystem = {InformationSystem:String}
                    AND Id < (
                    SELECT MAX(Id) AS LastId
                    FROM LogFiles lf
                    WHERE InformationSystem = {InformationSystem:String}
                )";
            commandRemoveArchiveLogInfo.Parameters.Add(new ClickHouseDbParameter
            {
                ParameterName = "InformationSystem",
                DbType = DbType.AnsiString,
                Value = system.Name
            });
            commandRemoveArchiveLogInfo.ExecuteNonQuery();
        }

        #endregion

        public void Dispose()
        {
            if (_connection != null)
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();

                _connection.Dispose();
                _connection = null;
            }
        }

        #endregion

        #region Private Methods

        private void CheckDatabaseSettings(string connectionSettings)
        {
            var connectionParams = ClickHouseHelpers.GetConnectionParams(connectionSettings);
            var databaseParam = connectionParams.FirstOrDefault(e => e.Key.ToUpper() == "DATABASE");
            _databaseName = databaseParam.Value;

            ClickHouseHelpers.CreateDatabaseIfNotExist(connectionSettings);
        }

        #endregion
    }
}

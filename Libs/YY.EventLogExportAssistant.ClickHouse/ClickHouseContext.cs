using ClickHouse.Ado;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using YY.EventLogExportAssistant.ClickHouse.Models;
using YY.EventLogExportAssistant.Database;
using YY.EventLogExportAssistant.Database.Models;
using YY.EventLogReaderAssistant;
using RowData = YY.EventLogReaderAssistant.Models.RowData;

namespace YY.EventLogExportAssistant.ClickHouse
{
    public class ClickHouseContext : IDisposable
    {
        #region Private Members

        private ClickHouseConnection _connection;
        private long logFileLastId = -1;
        private long applicationLastId = -1;
        private long computerLastId  = -1;
        private long eventLastId = -1;
        private long metadataLastId = -1;
        private long primaryPortLastId = -1;
        private long secondaryPortLastId = -1;
        private long severityPortLastId = -1;
        private long transactionStatusPortLastId = -1;
        private long userLastId = -1;
        private long workServerLastId = -1;

        #endregion

        #region Constructors

        public ClickHouseContext(ClickHouseConnectionSettings connectionSettings)
        {
            _connection = new ClickHouseConnection(connectionSettings);
            _connection.Open();

            var cmdDDL = _connection.CreateCommand();
            cmdDDL.CommandText =
                @"create table if not exists Applications
                (
	                InformationSystemId Int64,
	                Id Int64,
	                Name String,
	                Presentation String
                )
                engine = MergeTree()                
                PRIMARY KEY (InformationSystemId, Id)
                ORDER BY (InformationSystemId, Id)
                SETTINGS index_granularity = 8192;";
            cmdDDL.ExecuteNonQuery();

            cmdDDL.CommandText =
                @"create table if not exists Computers
                (
	                InformationSystemId Int64,
	                Id Int64,
	                Name String
                )
                engine = MergeTree()
                PRIMARY KEY (InformationSystemId, Id)
                ORDER BY (InformationSystemId, Id)
                SETTINGS index_granularity = 8192;";
            cmdDDL.ExecuteNonQuery();

            cmdDDL.CommandText =
                @"create table if not exists Events
                (
	                InformationSystemId Int64,
	                Id Int64,
	                Name String,
	                Presentation String
                )
                engine = MergeTree()
                PRIMARY KEY (InformationSystemId, Id)
                ORDER BY (InformationSystemId, Id)
                SETTINGS index_granularity = 8192;";
            cmdDDL.ExecuteNonQuery();

            cmdDDL.CommandText =
                @"create table if not exists InformationSystems
                (
	                Id Int64,
	                Name String,
	                Description String
                )
                engine = MergeTree()
                PRIMARY KEY Id
                ORDER BY Id
                SETTINGS index_granularity = 8192;";
            cmdDDL.ExecuteNonQuery();

            cmdDDL.CommandText =
                @"create table if not exists Metadata
                (
	                InformationSystemId Int64,
	                Id Int64,
	                Name String,
	                Uuid UUID
                )
                engine = MergeTree()
                PRIMARY KEY (InformationSystemId, Id)
                ORDER BY (InformationSystemId, Id)
                SETTINGS index_granularity = 8192;";
            cmdDDL.ExecuteNonQuery();

            cmdDDL.CommandText =
                @"create table if not exists PrimaryPorts
                (
	                InformationSystemId Int64,
	                Id Int64,
	                Name String
                )
                engine = MergeTree()
                PRIMARY KEY (InformationSystemId, Id)
                ORDER BY (InformationSystemId, Id)
                SETTINGS index_granularity = 8192;";
            cmdDDL.ExecuteNonQuery();

            cmdDDL.CommandText =
                @"create table if not exists SecondaryPorts
                (
	                InformationSystemId Int64,
	                Id Int64,
	                Name String
                )
                engine = MergeTree()
                PRIMARY KEY (InformationSystemId, Id)
                ORDER BY (InformationSystemId, Id)
                SETTINGS index_granularity = 8192;";
            cmdDDL.ExecuteNonQuery();

            cmdDDL.CommandText =
                @"create table if not exists Severities
                (
	                InformationSystemId Int64,
	                Id Int64,
	                Name String,
	                Presentation String
                )
                engine = MergeTree()
                PRIMARY KEY (InformationSystemId, Id)
                ORDER BY (InformationSystemId, Id)
                SETTINGS index_granularity = 8192;";
            cmdDDL.ExecuteNonQuery();

            cmdDDL.CommandText =
                @"create table if not exists TransactionStatuses
                (
	                InformationSystemId Int64,
	                Id Int64,
	                Name String,
	                Presentation String
                )
                engine = MergeTree()
                PRIMARY KEY (InformationSystemId, Id)
                ORDER BY (InformationSystemId, Id)
                SETTINGS index_granularity = 8192;";
            cmdDDL.ExecuteNonQuery();

            cmdDDL.CommandText =
                @"create table if not exists Users
                (
	                InformationSystemId Int64,
	                Id Int64,
	                Name String,
	                Uuid UUID
                )
                engine = MergeTree()
                PRIMARY KEY (InformationSystemId, Id)
                ORDER BY (InformationSystemId, Id)
                SETTINGS index_granularity = 8192;";
            cmdDDL.ExecuteNonQuery();

            cmdDDL.CommandText =
                @"create table if not exists WorkServers
                (
	                InformationSystemId Int64,
	                Id Int64,
	                Name String
                )
                engine = MergeTree()
                PRIMARY KEY (InformationSystemId, Id)
                ORDER BY (InformationSystemId, Id)
                SETTINGS index_granularity = 8192;";
            cmdDDL.ExecuteNonQuery();

            cmdDDL.CommandText =
                @"create table if not exists RowsData
                (
	                InformationSystemId Int64,
	                Id Int64,
	                Period DateTime,
	                SeverityId Int64,
	                ConnectId Int64,
	                Session Int64,
	                TransactionStatusId Int64,
	                TransactionDate DateTime,
	                TransactionId Int64,
	                UserId Int64,
	                ComputerId Int64,
	                ApplicationId Int64,
	                EventId Int64,
	                Comment String,
	                MetadataId Int64,
	                Data String,
	                DataUUID String,
	                DataPresentation String,
	                WorkServerId Int64,
	                PrimaryPortId Int64,
	                SecondaryPortId Int64
                )
                engine = MergeTree()
                PARTITION BY (InformationSystemId, toYYYYMM(Period))
                PRIMARY KEY (InformationSystemId, Period, Id)
                ORDER BY (InformationSystemId, Period, Id)
                SETTINGS index_granularity = 8192;";
            cmdDDL.ExecuteNonQuery();

            cmdDDL.CommandText =
                @"create table if not exists LogFiles
                (
	                InformationSystemId Int64,
	                Id Int64,
	                FileName String,
	                CreateDate DateTime,
	                ModificationDate DateTime,
	                LastEventNumber Int64,
	                LastCurrentFileReferences String,
	                LastCurrentFileData String,
	                LastStreamPosition Int64
                )
                engine = MergeTree()
                PRIMARY KEY (InformationSystemId, Id)
                ORDER BY (InformationSystemId, Id)
                SETTINGS index_granularity = 8192;";
            cmdDDL.ExecuteNonQuery();
        }

        #endregion

        #region Public Methods

        #region ReferencesCache

        public void FillReferencesCacheByDatabaseContext(long informationSystemId, ReferencesDataCache referencesDataCache)
        {
            referencesDataCache.Applications = GetAllApplications(informationSystemId);
            referencesDataCache.Computers = GetAllComputers(informationSystemId);
            referencesDataCache.Events = GetAllEvents(informationSystemId);
            referencesDataCache.Metadata = GetAllMetadata(informationSystemId);
            referencesDataCache.PrimaryPorts = GetAllPrimaryPorts(informationSystemId);
            referencesDataCache.SecondaryPorts = GetAllSecondaryPorts(informationSystemId);
            referencesDataCache.Severities = GetAllSeverities(informationSystemId);
            referencesDataCache.TransactionStatuses = GetAllTransactionStatuses(informationSystemId);
            referencesDataCache.Users = GetAllUsers(informationSystemId);
            referencesDataCache.WorkServers = GetAllWorkServers(informationSystemId);

            referencesDataCache.ApplicationsDictionary = referencesDataCache.Applications.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            referencesDataCache.ComputersDictionary = referencesDataCache.Computers.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            referencesDataCache.EventsDictionary = referencesDataCache.Events.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            referencesDataCache.MetadataDictionary = referencesDataCache.Metadata.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            referencesDataCache.PrimaryPortsDictionary = referencesDataCache.PrimaryPorts.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            referencesDataCache.SecondaryPortsDictionary = referencesDataCache.SecondaryPorts.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            referencesDataCache.SeveritiesDictionary = referencesDataCache.Severities.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            referencesDataCache.TransactionStatusesDictionary = referencesDataCache.TransactionStatuses.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            referencesDataCache.UsersDictionary = referencesDataCache.Users.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
            referencesDataCache.WorkServersDictionary = referencesDataCache.WorkServers.GroupBy(e => e.Name).ToDictionary(e => e.Key, e => e.ToList());
        }

        #endregion

        #region RowsData

        public void SaveRowsData(List<RowDataBulkInsert> rowsData)
        {
            var commandBulk = _connection.CreateCommand();
            commandBulk.CommandText = @"INSERT INTO RowsData 
            (
                InformationSystemId,
                Id,
                Period,
                SeverityId,
                ConnectId,
                Session,
                TransactionStatusId,
                TransactionDate,
                TransactionId,
                UserId,
                ComputerId,
                ApplicationId,
                EventId,
                Comment,
                MetadataId,
                Data,
                DataUUID,
                DataPresentation,
                WorkServerId,
                PrimaryPortId,
                SecondaryPortId
            ) VALUES @bulk";
            commandBulk.Parameters.Add(new ClickHouseParameter
            {
                ParameterName = "bulk",
                Value = rowsData
            });
            commandBulk.ExecuteNonQuery();
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
                    WHERE InformationSystemId = @InformationSystemId ";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    Value = system.Id
                });
                using (var cmdReader = command.ExecuteReader())
                {
                    if (cmdReader.NextResult() && cmdReader.Read())
                        output = cmdReader.GetDateTime(0);
                };
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
                        InformationSystemId,
                        Id,
                        Period
                    FROM RowsData AS RD
                    WHERE InformationSystemId = @existInfSysId
                        AND Id = @existId
                        AND Period = @existPeriod";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "existInfSysId",
                    DbType = DbType.Int64,
                    Value = system.Id
                });
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "existId",
                    DbType = DbType.Int64,
                    Value = rowData.RowId
                });
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "existPeriod",
                    DbType = DbType.DateTime,
                    Value = rowData.Period.DateTime
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    if (cmdReader.NextResult() && cmdReader.Read())
                        output = true;
                };
            }

            return output;
        }

        #endregion

        #region LogFiles

        public EventLogPosition GetLogFilePosition(long informationSystemId)
        {
            var cmdGetLastLogFileInfo = _connection.CreateCommand();
            cmdGetLastLogFileInfo.CommandText =
                @"SELECT	                
	                LastEventNumber,
	                LastCurrentFileReferences,
	                LastCurrentFileData,
	                LastStreamPosition
                FROM LogFiles AS LF
                WHERE InformationSystemId = @informationSystemId
                    AND Id IN (
                        SELECT
                            MAX(Id) LastId
                        FROM LogFiles AS LF_LAST
                        WHERE LF_LAST.InformationSystemId = @informationSystemId
                    )";
            cmdGetLastLogFileInfo.Parameters.Add(new ClickHouseParameter
            {
                ParameterName = "informationSystemId",
                Value = informationSystemId
            });

            EventLogPosition output = null;
            using (var cmdReader = cmdGetLastLogFileInfo.ExecuteReader())
            {
                if (cmdReader.NextResult() && cmdReader.Read())
                    output = new EventLogPosition(
                        cmdReader.GetInt64(0),
                        cmdReader.GetString(1),
                        cmdReader.GetString(2),
                        cmdReader.GetInt64(3));
            }

            return output;
        }
        public void SaveLogPosition(InformationSystemsBase system, FileInfo logFileInfo, EventLogPosition position)
        {
            var commandAddLogInfo = _connection.CreateCommand();
            commandAddLogInfo.CommandText =
                @"INSERT INTO LogFiles (
	                InformationSystemId,
	                Id,
	                FileName,
	                CreateDate,
	                ModificationDate,
	                LastEventNumber,
	                LastCurrentFileReferences,
	                LastCurrentFileData,
	                LastStreamPosition
                ) VALUES (
                    @isId,
	                @newId,
	                @FileName,
	                @CreateDate,
	                @ModificationDate,
	                @LastEventNumber,
	                @LastCurrentFileReferences,
	                @LastCurrentFileData,
	                @LastStreamPosition     
                )";

            commandAddLogInfo.Parameters.Add(new ClickHouseParameter
            {
                ParameterName = "isId",
                DbType = DbType.Int64,
                Value = system.Id
            });
            commandAddLogInfo.Parameters.Add(new ClickHouseParameter
            {
                ParameterName = "newId",
                DbType = DbType.Int64,
                Value = GetLogFileInfoNewId(system.Id)
            });
            commandAddLogInfo.Parameters.Add(new ClickHouseParameter
            {
                ParameterName = "FileName",
                DbType = DbType.AnsiString,
                Value = logFileInfo.Name
            });
            commandAddLogInfo.Parameters.Add(new ClickHouseParameter
            {
                ParameterName = "CreateDate",
                DbType = DbType.DateTime,
                Value = logFileInfo.CreationTimeUtc
            });
            commandAddLogInfo.Parameters.Add(new ClickHouseParameter
            {
                ParameterName = "ModificationDate",
                DbType = DbType.DateTime,
                Value = logFileInfo.LastWriteTimeUtc
            });
            commandAddLogInfo.Parameters.Add(new ClickHouseParameter
            {
                ParameterName = "LastEventNumber",
                DbType = DbType.Int64,
                Value = position.EventNumber
            });
            commandAddLogInfo.Parameters.Add(new ClickHouseParameter
            {
                ParameterName = "LastCurrentFileReferences",
                DbType = DbType.AnsiString,
                Value = position.CurrentFileReferences
            });
            commandAddLogInfo.Parameters.Add(new ClickHouseParameter
            {
                ParameterName = "LastCurrentFileData",
                DbType = DbType.AnsiString,
                Value = position.CurrentFileData
            });
            commandAddLogInfo.Parameters.Add(new ClickHouseParameter
            {
                ParameterName = "LastStreamPosition",
                DbType = DbType.Int64,
                Value = position.StreamPosition
            });

            commandAddLogInfo.ExecuteNonQuery();
        }
        public long GetLogFileInfoNewId(long informationSystemId)
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
                    WHERE InformationSystemId = @InformationSystemId ";
                    command.Parameters.Add(new ClickHouseParameter
                    {
                        ParameterName = "InformationSystemId",
                        Value = informationSystemId
                    });
                    using (var cmdReader = command.ExecuteReader())
                    {
                        if (cmdReader.NextResult() && cmdReader.Read())
                            output = cmdReader.GetInt64(0);
                    }

                    ;
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

        #endregion

        #region InformationSystem

        public InformationSystems CreateOrUpdateInformationSystem(string name, string description)
        {
            var existItem = GetInformationSystemByName(name);
            if (existItem == null)
            {
                var commandAdd = _connection.CreateCommand();
                commandAdd.CommandText = "INSERT INTO InformationSystems (Id,Name,Description) VALUES (@Id,@Name,@Description)";
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Id",
                    DbType = DbType.Int64,
                    Value = GetInformationSystemNewId()
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = name
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Description",
                    DbType = DbType.AnsiString,
                    Value = description
                });
                commandAdd.ExecuteNonQuery();
                existItem = GetInformationSystemByName(name);
            }
            else
            {
                if (existItem.Description != description)
                {
                    var commandUpdate = _connection.CreateCommand();
                    commandUpdate.CommandText =
                        @"ALTER TABLE InformationSystems
                        UPDATE Description = @Description
                        WHERE Id = @Id";
                    commandUpdate.Parameters.Add(new ClickHouseParameter
                    {
                        ParameterName = "Id",
                        DbType = DbType.Int64,
                        Value = existItem.Id
                    });
                    commandUpdate.Parameters.Add(new ClickHouseParameter
                    {
                        ParameterName = "Description",
                        DbType = DbType.AnsiString,
                        Value = description
                    });
                    commandUpdate.ExecuteNonQuery();
                }
            }

            return existItem;
        }
        public InformationSystems GetInformationSystemByName(string name)
        {
            InformationSystems output = null;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name,
                        Description
                    FROM InformationSystems as IS
                    WHERE IS.Name = @Name";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    Value = name
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    if (cmdReader.NextResult() && cmdReader.Read())
                        output = new InformationSystems()
                        {
                            Id = cmdReader.GetInt64(0),
                            Name = cmdReader.GetString(1),
                            Description = cmdReader.GetString(2)
                        };
                }
            }

            return output;
        }
        public long GetInformationSystemNewId()
        {
            long output = 0;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        MAX(Id)
                    FROM InformationSystems as IS";
                using (var cmdReader = command.ExecuteReader())
                {
                    if (cmdReader.NextResult() && cmdReader.Read())
                        output = cmdReader.GetInt64(0);
                };
            }

            output += 1;

            return output;
        }

        #endregion

        #region Application

        public void AddApplicationIfNotExist(long informationSystemId, EventLogReaderAssistant.Models.Applications sourceItem)
        {
            var existItem = GetApplicationByName(informationSystemId, sourceItem.Name);
            if (existItem == null)
            {
                var commandAdd = _connection.CreateCommand();
                commandAdd.CommandText = "INSERT INTO Applications (InformationSystemId,Id,Name,Presentation) VALUES (@InformationSystemId,@Id,@Name,@Presentation)";
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Id",
                    DbType = DbType.Int64,
                    Value = GetApplicationNewId(informationSystemId)
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = sourceItem.Name
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Presentation",
                    DbType = DbType.AnsiString,
                    Value = Database.Models.Applications.GetPresentationByName(sourceItem.Name)
                });
                commandAdd.ExecuteNonQuery();
            }
        }
        public Applications GetApplicationByName(long informationSystemId, string name)
        {
            Applications output = null;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name,
                        Presentation
                    FROM Applications as APPS
                    WHERE InformationSystemId = @InformationSystemId
                        AND Name = @Name";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = name
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    if (cmdReader.NextResult() && cmdReader.Read())
                        output = new Applications()
                        {
                            InformationSystemId = informationSystemId,
                            Id = cmdReader.GetInt64(0),
                            Name = cmdReader.GetString(1),
                            Presentation = cmdReader.GetString(2)
                        };
                }
            }

            return output;
        }
        public List<Applications> GetAllApplications(long informationSystemId)
        {
            List<Applications> output = new List<Applications>();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name,
                        Presentation
                    FROM Applications as APPS
                    WHERE InformationSystemId = @InformationSystemId";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    while (cmdReader.NextResult())
                        while (cmdReader.Read())
                        {
                            output.Add(new Applications()
                            {
                                InformationSystemId = informationSystemId,
                                Id = cmdReader.GetInt64(0),
                                Name = cmdReader.GetString(1),
                                Presentation = cmdReader.GetString(2)
                            });
                        }
                }
            }

            return output;
        }
        public long GetApplicationNewId(long informationSystemId)
        {
            long output = 0;

            if (applicationLastId < 0)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText =
                        @"SELECT 
                            MAX(t.Id) AS LastId
                        FROM Applications t
                        WHERE InformationSystemId = @InformationSystemId ";
                    command.Parameters.Add(new ClickHouseParameter
                    {
                        ParameterName = "InformationSystemId",
                        Value = informationSystemId
                    });
                    using (var cmdReader = command.ExecuteReader())
                    {
                        if (cmdReader.NextResult() && cmdReader.Read())
                            output = cmdReader.GetInt64(0);
                    }
                }
            }
            else
            {
                output = applicationLastId;
            }

            output += 1;
            applicationLastId = output;

            return output;
        }

        #endregion

        #region Computer

        public void AddComputerIfNotExist(long informationSystemId, EventLogReaderAssistant.Models.Computers sourceItem)
        {
            var existItem = GetComputerByName(informationSystemId, sourceItem.Name);
            if (existItem == null)
            {
                var commandAdd = _connection.CreateCommand();
                commandAdd.CommandText = "INSERT INTO Computers (InformationSystemId,Id,Name) VALUES (@InformationSystemId,@Id,@Name)";
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Id",
                    DbType = DbType.Int64,
                    Value = GetComputerNewId(informationSystemId)
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = sourceItem.Name
                });
                commandAdd.ExecuteNonQuery();
            }
        }
        public Computers GetComputerByName(long informationSystemId, string name)
        {
            Computers output = null;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name                        
                    FROM Computers as CMPS
                    WHERE InformationSystemId = @InformationSystemId
                        AND Name = @Name";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = name
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    if (cmdReader.NextResult() && cmdReader.Read())
                        output = new Computers()
                        {
                            InformationSystemId = informationSystemId,
                            Id = cmdReader.GetInt64(0),
                            Name = cmdReader.GetString(1)
                        };
                }
            }

            return output;
        }
        public List<Computers> GetAllComputers(long informationSystemId)
        {
            List<Computers> output = new List<Computers>();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name                        
                    FROM Computers as CMPS
                    WHERE InformationSystemId = @InformationSystemId";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    while (cmdReader.NextResult())
                        while (cmdReader.Read())
                        {
                            output.Add(new Computers()
                            {
                                InformationSystemId = informationSystemId,
                                Id = cmdReader.GetInt64(0),
                                Name = cmdReader.GetString(1)
                            });
                        }
                }
            }

            return output;
        }
        public long GetComputerNewId(long informationSystemId)
        {
            long output = 0;

            if (computerLastId < 0)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText =
                        @"SELECT 
                        MAX(t.Id) AS LastId
                    FROM Computers t
                    WHERE InformationSystemId = @InformationSystemId ";
                    command.Parameters.Add(new ClickHouseParameter
                    {
                        ParameterName = "InformationSystemId",
                        Value = informationSystemId
                    });
                    using (var cmdReader = command.ExecuteReader())
                    {
                        if (cmdReader.NextResult() && cmdReader.Read())
                            output = cmdReader.GetInt64(0);
                    }

                    ;
                }
            }
            else
            {
                output = computerLastId;
            }

            output += 1;
            computerLastId = output;

            return output;
        }

        #endregion

        #region Event

        public void AddEventIfNotExist(long informationSystemId, EventLogReaderAssistant.Models.Events sourceItem)
        {
            var existItem = GetEventByName(informationSystemId, sourceItem.Name);
            if (existItem == null)
            {
                var commandAdd = _connection.CreateCommand();
                commandAdd.CommandText = "INSERT INTO Events (InformationSystemId,Id,Name,Presentation) VALUES (@InformationSystemId,@Id,@Name,@Presentation)";
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Id",
                    DbType = DbType.Int64,
                    Value = GetEventNewId(informationSystemId)
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = sourceItem.Name
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Presentation",
                    DbType = DbType.AnsiString,
                    Value = Database.Models.Events.GetPresentationByName(sourceItem.Name)
                });
                commandAdd.ExecuteNonQuery();
            }
        }
        public Events GetEventByName(long informationSystemId, string name)
        {
            Events output = null;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name,
                        Presentation
                    FROM Events as EVNTS
                    WHERE InformationSystemId = @InformationSystemId
                        AND Name = @Name";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = name
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    if (cmdReader.NextResult() && cmdReader.Read())
                        output = new Events()
                        {
                            InformationSystemId = informationSystemId,
                            Id = cmdReader.GetInt64(0),
                            Name = cmdReader.GetString(1),
                            Presentation = cmdReader.GetString(2)
                        };
                }
            }

            return output;
        }
        public List<Events> GetAllEvents(long informationSystemId)
        {
            List<Events> output = new List<Events>();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name,
                        Presentation
                    FROM Events as EVNTS
                    WHERE InformationSystemId = @InformationSystemId";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    while (cmdReader.NextResult())
                        while (cmdReader.Read())
                        {
                            output.Add(new Events()
                            {
                                InformationSystemId = informationSystemId,
                                Id = cmdReader.GetInt64(0),
                                Name = cmdReader.GetString(1),
                                Presentation = cmdReader.GetString(2)
                            });
                        }
                }
            }

            return output;
        }
        public long GetEventNewId(long informationSystemId)
        {
            long output = 0;

            if (eventLastId < 0)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText =
                        @"SELECT 
                        MAX(t.Id) AS LastId
                    FROM Events t
                    WHERE InformationSystemId = @InformationSystemId ";
                    command.Parameters.Add(new ClickHouseParameter
                    {
                        ParameterName = "InformationSystemId",
                        Value = informationSystemId
                    });
                    using (var cmdReader = command.ExecuteReader())
                    {
                        if (cmdReader.NextResult() && cmdReader.Read())
                            output = cmdReader.GetInt64(0);
                    }

                    ;
                }
            }
            else
            {
                output = eventLastId;
            }

            output += 1;
            eventLastId = output;

            return output;
        }

        #endregion

        #region Metadata

        public void AddMetadataIfNotExist(long informationSystemId, EventLogReaderAssistant.Models.Metadata sourceItem)
        {
            var existItem = GetMetadataByParams(informationSystemId, sourceItem.Name, sourceItem.Uuid);
            if (existItem == null)
            {
                var commandAdd = _connection.CreateCommand();
                commandAdd.CommandText = "INSERT INTO Metadata (InformationSystemId,Id,Name,Uuid) VALUES (@InformationSystemId,@Id,@Name,@Uuid)";
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Id",
                    DbType = DbType.Int64,
                    Value = GetMetadataNewId(informationSystemId)
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = sourceItem.Name
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Uuid",
                    DbType = DbType.Guid,
                    Value = sourceItem.Uuid
                });
                commandAdd.ExecuteNonQuery();
            }
        }
        public Metadata GetMetadataByParams(long informationSystemId, string name, Guid uuid)
        {
            Metadata output = null;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name,
                        Uuid
                    FROM Metadata as MT
                    WHERE InformationSystemId = @InformationSystemId
                        AND Name = @Name
                        AND Uuid = @Uuid";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = name
                });
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Uuid",
                    DbType = DbType.Guid,
                    Value = uuid
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    if (cmdReader.NextResult() && cmdReader.Read())
                        output = new Metadata()
                        {
                            InformationSystemId = informationSystemId,
                            Id = cmdReader.GetInt64(0),
                            Name = cmdReader.GetString(1),
                            Uuid = cmdReader.GetGuid(2)
                        };
                }
            }

            return output;
        }
        public List<Metadata> GetAllMetadata(long informationSystemId)
        {
            List<Metadata> output = new List<Metadata>();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name,
                        Uuid
                    FROM Metadata as MT
                    WHERE InformationSystemId = @InformationSystemId";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    while (cmdReader.NextResult())
                        while (cmdReader.Read())
                        {
                            output.Add(new Metadata()
                            {
                                InformationSystemId = informationSystemId,
                                Id = cmdReader.GetInt64(0),
                                Name = cmdReader.GetString(1),
                                Uuid = cmdReader.GetGuid(2)
                            });
                        }
                }
            }

            return output;
        }
        public long GetMetadataNewId(long informationSystemId)
        {
            long output = 0;

            if (metadataLastId < 0)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText =
                        @"SELECT 
                        MAX(t.Id) AS LastId
                    FROM Metadata t
                    WHERE InformationSystemId = @InformationSystemId ";
                    command.Parameters.Add(new ClickHouseParameter
                    {
                        ParameterName = "InformationSystemId",
                        Value = informationSystemId
                    });
                    using (var cmdReader = command.ExecuteReader())
                    {
                        if (cmdReader.NextResult() && cmdReader.Read())
                            output = cmdReader.GetInt64(0);
                    }
                }
            }
            else
            {
                output = metadataLastId;
            }

            output += 1;
            metadataLastId = output;

            return output;
        }

        #endregion

        #region PrimaryPort

        public void AddPrimaryPortIfNotExist(long informationSystemId, EventLogReaderAssistant.Models.PrimaryPorts sourceItem)
        {
            var existItem = GetPrimaryPortByName(informationSystemId, sourceItem.Name);
            if (existItem == null)
            {
                var commandAdd = _connection.CreateCommand();
                commandAdd.CommandText = "INSERT INTO PrimaryPorts (InformationSystemId,Id,Name) VALUES (@InformationSystemId,@Id,@Name)";
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Id",
                    DbType = DbType.Int64,
                    Value = GetPrimaryPortNewId(informationSystemId)
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = sourceItem.Name
                });
                commandAdd.ExecuteNonQuery();
            }
        }
        public PrimaryPorts GetPrimaryPortByName(long informationSystemId, string name)
        {
            PrimaryPorts output = null;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name                        
                    FROM PrimaryPorts as PP
                    WHERE InformationSystemId = @InformationSystemId
                        AND Name = @Name";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = name
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    if (cmdReader.NextResult() && cmdReader.Read())
                        output = new PrimaryPorts()
                        {
                            InformationSystemId = informationSystemId,
                            Id = cmdReader.GetInt64(0),
                            Name = cmdReader.GetString(1)
                        };
                }
            }

            return output;
        }
        public List<PrimaryPorts> GetAllPrimaryPorts(long informationSystemId)
        {
            List<PrimaryPorts> output = new List<PrimaryPorts>();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name                        
                    FROM PrimaryPorts as PP
                    WHERE InformationSystemId = @InformationSystemId";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    while (cmdReader.NextResult())
                        while (cmdReader.Read())
                        {
                            output.Add(new PrimaryPorts()
                            {
                                InformationSystemId = informationSystemId,
                                Id = cmdReader.GetInt64(0),
                                Name = cmdReader.GetString(1)
                            });
                        }
                }
            }

            return output;
        }
        public long GetPrimaryPortNewId(long informationSystemId)
        {
            long output = 0;

            if (primaryPortLastId < 0)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText =
                        @"SELECT 
                        MAX(t.Id) AS LastId
                    FROM PrimaryPorts t
                    WHERE InformationSystemId = @InformationSystemId ";
                    command.Parameters.Add(new ClickHouseParameter
                    {
                        ParameterName = "InformationSystemId",
                        Value = informationSystemId
                    });
                    using (var cmdReader = command.ExecuteReader())
                    {
                        if (cmdReader.NextResult() && cmdReader.Read())
                            output = cmdReader.GetInt64(0);
                    }

                    ;
                }
            }
            else
            {
                output = primaryPortLastId;
            }

            output += 1;
            primaryPortLastId = output;

            return output;
        }

        #endregion

        #region SecondaryPort

        public void AddSecondaryPortIfNotExist(long informationSystemId, EventLogReaderAssistant.Models.SecondaryPorts sourceItem)
        {
            var existItem = GetSecondaryPortByName(informationSystemId, sourceItem.Name);
            if (existItem == null)
            {
                var commandAdd = _connection.CreateCommand();
                commandAdd.CommandText = "INSERT INTO SecondaryPorts (InformationSystemId,Id,Name) VALUES (@InformationSystemId,@Id,@Name)";
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Id",
                    DbType = DbType.Int64,
                    Value = GetSecondaryPortNewId(informationSystemId)
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = sourceItem.Name
                });
                commandAdd.ExecuteNonQuery();
            }
        }
        public SecondaryPorts GetSecondaryPortByName(long informationSystemId, string name)
        {
            SecondaryPorts output = null;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name                        
                    FROM SecondaryPorts as SP
                    WHERE InformationSystemId = @InformationSystemId
                        AND Name = @Name";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = name
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    if (cmdReader.NextResult() && cmdReader.Read())
                        output = new SecondaryPorts()
                        {
                            InformationSystemId = informationSystemId,
                            Id = cmdReader.GetInt64(0),
                            Name = cmdReader.GetString(1)
                        };
                }
            }

            return output;
        }
        public List<SecondaryPorts> GetAllSecondaryPorts(long informationSystemId)
        {
            List<SecondaryPorts> output = new List<SecondaryPorts>();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name                        
                    FROM SecondaryPorts as SP
                    WHERE InformationSystemId = @InformationSystemId";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    while (cmdReader.NextResult())
                        while (cmdReader.Read())
                        {
                            output.Add(new SecondaryPorts()
                            {
                                InformationSystemId = informationSystemId,
                                Id = cmdReader.GetInt64(0),
                                Name = cmdReader.GetString(1)
                            });
                        }
                }
            }

            return output;
        }
        public long GetSecondaryPortNewId(long informationSystemId)
        {
            long output = 0;

            if (secondaryPortLastId < 0)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText =
                        @"SELECT 
                        MAX(t.Id) AS LastId
                    FROM SecondaryPorts t
                    WHERE InformationSystemId = @InformationSystemId ";
                    command.Parameters.Add(new ClickHouseParameter
                    {
                        ParameterName = "InformationSystemId",
                        Value = informationSystemId
                    });
                    using (var cmdReader = command.ExecuteReader())
                    {
                        if (cmdReader.NextResult() && cmdReader.Read())
                            output = cmdReader.GetInt64(0);
                    }

                    ;
                }
            }
            else
            {
                output = secondaryPortLastId;
            }

            output += 1;
            secondaryPortLastId = output;

            return output;
        }

        #endregion

        #region Severity

        public void AddSeverityIfNotExist(long informationSystemId, EventLogReaderAssistant.Models.Severity sourceItem)
        {
            var existItem = GetSeverityByName(informationSystemId, sourceItem.ToString());
            if (existItem == null)
            {
                var commandAdd = _connection.CreateCommand();
                commandAdd.CommandText = "INSERT INTO Severities (InformationSystemId,Id,Name,Presentation) VALUES (@InformationSystemId,@Id,@Name,@Presentation)";
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Id",
                    DbType = DbType.Int64,
                    Value = GetSeverityNewId(informationSystemId)
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = sourceItem.ToString()
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Presentation",
                    DbType = DbType.AnsiString,
                    Value = Database.Models.Severities.GetPresentationByName(sourceItem.ToString())
                });
                commandAdd.ExecuteNonQuery();
            }
        }
        public Severities GetSeverityByName(long informationSystemId, string name)
        {
            Severities output = null;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name,
                        Presentation
                    FROM Severities as SV
                    WHERE InformationSystemId = @InformationSystemId
                        AND Name = @Name";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = name
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    if (cmdReader.NextResult() && cmdReader.Read())
                        output = new Severities()
                        {
                            InformationSystemId = informationSystemId,
                            Id = cmdReader.GetInt64(0),
                            Name = cmdReader.GetString(1),
                            Presentation = cmdReader.GetString(2)
                        };
                }
            }

            return output;
        }
        public List<Severities> GetAllSeverities(long informationSystemId)
        {
            List<Severities> output = new List<Severities>();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name,
                        Presentation
                    FROM Severities as SV
                    WHERE InformationSystemId = @InformationSystemId";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    while (cmdReader.NextResult())
                        while (cmdReader.Read())
                        {
                            output.Add(new Severities()
                            {
                                InformationSystemId = informationSystemId,
                                Id = cmdReader.GetInt64(0),
                                Name = cmdReader.GetString(1),
                                Presentation = cmdReader.GetString(2)
                            });
                        }
                }
            }

            return output;
        }
        public long GetSeverityNewId(long informationSystemId)
        {
            long output = 0;

            if (severityPortLastId < 0)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText =
                        @"SELECT 
                        MAX(t.Id) AS LastId
                    FROM Severities t
                    WHERE InformationSystemId = @InformationSystemId ";
                    command.Parameters.Add(new ClickHouseParameter
                    {
                        ParameterName = "InformationSystemId",
                        Value = informationSystemId
                    });
                    using (var cmdReader = command.ExecuteReader())
                    {
                        if (cmdReader.NextResult() && cmdReader.Read())
                            output = cmdReader.GetInt64(0);
                    }

                    ;
                }
            }
            else
            {
                output = severityPortLastId;
            }

            output += 1;
            severityPortLastId = output;

            return output;
        }

        #endregion

        #region TransactionStatus

        public void AddTransactionStatusIfNotExist(long informationSystemId, EventLogReaderAssistant.Models.TransactionStatus sourceItem)
        {
            var existItem = GetTransactionStatusByName(informationSystemId, sourceItem.ToString());
            if (existItem == null)
            {
                var commandAdd = _connection.CreateCommand();
                commandAdd.CommandText = "INSERT INTO TransactionStatuses (InformationSystemId,Id,Name,Presentation) VALUES (@InformationSystemId,@Id,@Name,@Presentation)";
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Id",
                    DbType = DbType.Int64,
                    Value = GetTransactionStatusNewId(informationSystemId)
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = sourceItem.ToString()
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Presentation",
                    DbType = DbType.AnsiString,
                    Value = Database.Models.TransactionStatuses.GetPresentationByName(sourceItem.ToString())
                });
                commandAdd.ExecuteNonQuery();
            }
        }
        public TransactionStatuses GetTransactionStatusByName(long informationSystemId, string name)
        {
            TransactionStatuses output = null;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name,
                        Presentation
                    FROM TransactionStatuses as TS
                    WHERE InformationSystemId = @InformationSystemId
                        AND Name = @Name";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = name
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    if (cmdReader.NextResult() && cmdReader.Read())
                        output = new TransactionStatuses()
                        {
                            InformationSystemId = informationSystemId,
                            Id = cmdReader.GetInt64(0),
                            Name = cmdReader.GetString(1),
                            Presentation = cmdReader.GetString(2)
                        };
                }
            }

            return output;
        }
        public List<TransactionStatuses> GetAllTransactionStatuses(long informationSystemId)
        {
            List<TransactionStatuses> output = new List<TransactionStatuses>();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name,
                        Presentation
                    FROM TransactionStatuses as TS
                    WHERE InformationSystemId = @InformationSystemId";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    while (cmdReader.NextResult())
                        while (cmdReader.Read())
                        {
                            output.Add(new TransactionStatuses()
                            {
                                InformationSystemId = informationSystemId,
                                Id = cmdReader.GetInt64(0),
                                Name = cmdReader.GetString(1),
                                Presentation = cmdReader.GetString(2)
                            });
                        }
                }
            }

            return output;
        }
        public long GetTransactionStatusNewId(long informationSystemId)
        {
            long output = 0;

            if (transactionStatusPortLastId < 0)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText =
                        @"SELECT 
                        MAX(t.Id) AS LastId
                    FROM TransactionStatuses t
                    WHERE InformationSystemId = @InformationSystemId ";
                    command.Parameters.Add(new ClickHouseParameter
                    {
                        ParameterName = "InformationSystemId",
                        Value = informationSystemId
                    });
                    using (var cmdReader = command.ExecuteReader())
                    {
                        if (cmdReader.NextResult() && cmdReader.Read())
                            output = cmdReader.GetInt64(0);
                    }

                    ;
                }
            }
            else
            {
                output = transactionStatusPortLastId;
            }

            output += 1;
            transactionStatusPortLastId = output;

            return output;
        }

        #endregion

        #region Users

        public void AddUserIfNotExist(long informationSystemId, EventLogReaderAssistant.Models.Users sourceItem)
        {
            var existItem = GetUserByParams(informationSystemId, sourceItem.Name, sourceItem.Uuid);
            if (existItem == null)
            {
                var commandAdd = _connection.CreateCommand();
                commandAdd.CommandText = "INSERT INTO Users (InformationSystemId,Id,Name,Uuid) VALUES (@InformationSystemId,@Id,@Name,@Uuid)";
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Id",
                    DbType = DbType.Int64,
                    Value = GetUserNewId(informationSystemId)
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = sourceItem.Name
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Uuid",
                    DbType = DbType.Guid,
                    Value = sourceItem.Uuid
                });
                commandAdd.ExecuteNonQuery();
            }
        }
        public Users GetUserByParams(long informationSystemId, string name, Guid uuid)
        {
            Users output = null;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name,
                        Uuid
                    FROM Users as USR
                    WHERE InformationSystemId = @InformationSystemId
                        AND Name = @Name
                        AND Uuid = @Uuid";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = name
                });
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Uuid",
                    DbType = DbType.Guid,
                    Value = uuid
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    if (cmdReader.NextResult() && cmdReader.Read())
                        output = new Users()
                        {
                            InformationSystemId = informationSystemId,
                            Id = cmdReader.GetInt64(0),
                            Name = cmdReader.GetString(1),
                            Uuid = cmdReader.GetGuid(2)
                        };
                }
            }

            return output;
        }
        public List<Users> GetAllUsers(long informationSystemId)
        {
            List<Users> output = new List<Users>();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name,
                        Uuid
                    FROM Users as USR
                    WHERE InformationSystemId = @InformationSystemId";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    while (cmdReader.NextResult())
                        while (cmdReader.Read())
                        {
                            output.Add(new Users()
                            {
                                InformationSystemId = informationSystemId,
                                Id = cmdReader.GetInt64(0),
                                Name = cmdReader.GetString(1),
                                Uuid = cmdReader.GetGuid(2)
                            });
                        }
                }
            }

            return output;
        }
        public long GetUserNewId(long informationSystemId)
        {
            long output = 0;

            if (userLastId < 0)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText =
                        @"SELECT 
                        MAX(t.Id) AS LastId
                    FROM Users t
                    WHERE InformationSystemId = @InformationSystemId ";
                    command.Parameters.Add(new ClickHouseParameter
                    {
                        ParameterName = "InformationSystemId",
                        Value = informationSystemId
                    });
                    using (var cmdReader = command.ExecuteReader())
                    {
                        if (cmdReader.NextResult() && cmdReader.Read())
                            output = cmdReader.GetInt64(0);
                    }

                    ;
                }
            }
            else
            {
                output = userLastId;
            }

            output += 1;
            userLastId = output;

            return output;
        }

        #endregion

        #region WorkServer

        public void AddWorkServerIfNotExist(long informationSystemId, EventLogReaderAssistant.Models.WorkServers sourceItem)
        {
            var existItem = GetWorkServerByName(informationSystemId, sourceItem.Name);
            if (existItem == null)
            {
                var commandAdd = _connection.CreateCommand();
                commandAdd.CommandText = "INSERT INTO WorkServers (InformationSystemId,Id,Name) VALUES (@InformationSystemId,@Id,@Name)";
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Id",
                    DbType = DbType.Int64,
                    Value = GetWorkServerNewId(informationSystemId)
                });
                commandAdd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = sourceItem.Name
                });
                commandAdd.ExecuteNonQuery();
            }
        }
        public WorkServers GetWorkServerByName(long informationSystemId, string name)
        {
            WorkServers output = null;

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name                        
                    FROM WorkServers as WS
                    WHERE InformationSystemId = @InformationSystemId
                        AND Name = @Name";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "Name",
                    DbType = DbType.AnsiString,
                    Value = name
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    if (cmdReader.NextResult() && cmdReader.Read())
                        output = new WorkServers()
                        {
                            InformationSystemId = informationSystemId,
                            Id = cmdReader.GetInt64(0),
                            Name = cmdReader.GetString(1)
                        };
                }
            }

            return output;
        }
        public List<WorkServers> GetAllWorkServers(long informationSystemId)
        {
            List<WorkServers> output = new List<WorkServers>();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT
                        Id,
                        Name                        
                    FROM WorkServers as WS
                    WHERE InformationSystemId = @InformationSystemId";
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "InformationSystemId",
                    DbType = DbType.Int64,
                    Value = informationSystemId
                });

                using (var cmdReader = command.ExecuteReader())
                {
                    while (cmdReader.NextResult())
                        while (cmdReader.Read())
                        {
                            output.Add(new WorkServers()
                            {
                                InformationSystemId = informationSystemId,
                                Id = cmdReader.GetInt64(0),
                                Name = cmdReader.GetString(1)
                            });
                        }
                }
            }

            return output;
        }
        public long GetWorkServerNewId(long informationSystemId)
        {
            long output = 0;

            if (workServerLastId < 0)
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText =
                        @"SELECT 
                        MAX(t.Id) AS LastId
                    FROM WorkServers t
                    WHERE InformationSystemId = @InformationSystemId ";
                    command.Parameters.Add(new ClickHouseParameter
                    {
                        ParameterName = "InformationSystemId",
                        Value = informationSystemId
                    });
                    using (var cmdReader = command.ExecuteReader())
                    {
                        if (cmdReader.NextResult() && cmdReader.Read())
                            output = cmdReader.GetInt64(0);
                    }
                }
            }
            else
            {
                output = workServerLastId;
            }

            output += 1;
            workServerLastId = output;

            return output;
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
    }
}

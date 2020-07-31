using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using YY.EventLogExportAssistant.Database;

namespace YY.EventLogExportAssistant.SQLServer
{
    public sealed class EventLogSQLServerActions : IEventLogContextExtensionActions
    {
        public void AdditionalInitializationActions(DatabaseFacade database)
        {
            database.ExecuteSqlRaw(
				@"IF NOT EXISTS (
	                SELECT * FROM sys. objects 
	                WHERE object_id = OBJECT_ID('dbo.vw_EventLog') AND type = 'V')
                BEGIN
	                EXECUTE(
		                'CREATE VIEW [dbo].[vw_EventLog]
		                AS
		                SELECT
			                [RD].[InformationSystemId] AS [InformationSystemId],
			                [IS].[Name] AS [InformationSystemName],
			                [RD].[Period],
			                [RD].[Id] AS [RowId],
			                [RD].[SeverityId] AS [SeverityId],
			                [SV].[Name] AS [SeverityName],
			                [RD].[ConnectId] AS [Connection],
			                [RD].[Session],
			                [RD].[TransactionStatusId],
			                [RD].[TransactionDate],
			                [RD].[TransactionId],
			                [RD].[UserId] AS [UserId],
			                [USR].[Name] AS [UserName],
			                [USR].[Uuid] AS [UserUUID],
			                [RD].[ComputerId] AS [ComputerId],
			                [CMP].[Name] AS [ComputerName],
			                [RD].[Data],
			                [RD].[DataUUID],
			                [RD].[DataPresentation],
			                [RD].[Comment],
			                [RD].[ApplicationId] AS [ApplicationId],
			                [APPS].[Name] AS [ApplicationName],
			                [RD].[EventId] AS [EventId],
			                [EVNT].[Name] AS [EventName],
			                [RD].[MetadataId] AS [MetadataId],
			                [META].[Name] AS [MetadataName],
			                [META].[Uuid] AS [MetadataUUID],
			                [RD].[WorkServerId] AS [WorkServerId],
			                [WSRV].[Name] AS [WorkServerName],
			                [RD].[PrimaryPortId] AS [PrimaryPortId],
			                [PPRT].[Name] AS [PrimaryPortName],
			                [RD].[SecondaryPortId] AS [SecondaryPortId],
			                [SPRT].[Name] AS [SecondaryPortName]
		                FROM [dbo].[RowsData] AS [RD]
			                LEFT JOIN [dbo].[InformationSystems] AS [IS]
			                ON [RD].[InformationSystemId] = [IS].[Id]
			                LEFT JOIN [dbo].[Severities] AS [SV]
			                ON [RD].[InformationSystemId] = [SV].[InformationSystemId]
				                AND [RD].[SeverityId] = [SV].[Id]
			                LEFT JOIN [dbo].[Users] AS [USR]
			                ON [RD].[InformationSystemId] = [USR].[InformationSystemId]
				                AND [RD].[UserId] = [USR].[Id]
			                LEFT JOIN [dbo].[Computers] AS [CMP]
			                ON [RD].[InformationSystemId] = [CMP].[InformationSystemId]
				                AND [RD].[ComputerId] = [CMP].[Id]
			                LEFT JOIN [dbo].[Applications] AS [APPS]
			                ON [RD].[InformationSystemId] = [APPS].[InformationSystemId]
				                AND [RD].[ApplicationId] = [APPS].[Id]
			                LEFT JOIN [dbo].[Events] AS [EVNT]
			                ON [RD].[InformationSystemId] = [EVNT].[InformationSystemId]
				                AND [RD].[EventId] = [EVNT].[Id]
			                LEFT JOIN [dbo].[Metadata] AS [META]
			                ON [RD].[InformationSystemId] = [META].[InformationSystemId]
				                AND [RD].[MetadataId] = [META].[Id]
			                LEFT JOIN [dbo].[WorkServers] AS [WSRV]
			                ON [RD].[InformationSystemId] = [WSRV].[InformationSystemId]
				                AND [RD].[WorkServerId] = [WSRV].[Id]
			                LEFT JOIN [dbo].[PrimaryPorts] AS [PPRT]
			                ON [RD].[InformationSystemId] = [PPRT].[InformationSystemId]
				                AND [RD].[PrimaryPortId] = [PPRT].[Id]
			                LEFT JOIN [dbo].[SecondaryPorts] AS [SPRT]
			                ON [RD].[InformationSystemId] = [SPRT].[InformationSystemId]
				                AND [RD].[SecondaryPortId] = [SPRT].[Id]')
                END");
		}

        public void OnModelCreating(ModelBuilder modelBuilder, out bool standardBehaviorChanged)
        {
            standardBehaviorChanged = false;
        }

        public void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
			if (!optionsBuilder.IsConfigured)
            {
                IConfiguration Configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                string connectinString = Configuration.GetConnectionString("EventLogDatabase");
                optionsBuilder.UseSqlServer(connectinString);
            }
		}
    }
}

CREATE TABLE [dbo].[Applications](
	[InformationSystemId] [bigint] NOT NULL,
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NULL,
	[Presentation] [nvarchar](500) NULL,
 CONSTRAINT [PK_Applications] PRIMARY KEY CLUSTERED 
(
	[InformationSystemId] ASC,
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Computers](
	[InformationSystemId] [bigint] NOT NULL,
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NULL,
 CONSTRAINT [PK_Computers] PRIMARY KEY CLUSTERED 
(
	[InformationSystemId] ASC,
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Events](
	[InformationSystemId] [bigint] NOT NULL,
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NULL,
	[Presentation] [nvarchar](500) NULL,
 CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED 
(
	[InformationSystemId] ASC,
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[InformationSystems](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Description] [nvarchar](500) NULL,
 CONSTRAINT [PK_InformationSystems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Metadata](
	[InformationSystemId] [bigint] NOT NULL,
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NULL,
	[Uuid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Metadata] PRIMARY KEY CLUSTERED 
(
	[InformationSystemId] ASC,
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[PrimaryPorts](
	[InformationSystemId] [bigint] NOT NULL,
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NULL,
 CONSTRAINT [PK_PrimaryPorts] PRIMARY KEY CLUSTERED 
(
	[InformationSystemId] ASC,
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[RowsData](
	[InformationSystemId] [bigint] NOT NULL,
	[Id] [bigint] NOT NULL,
	[Period] [datetimeoffset](7) NOT NULL,
	[SeverityId] [bigint] NULL,
	[ConnectId] [bigint] NULL,
	[Session] [bigint] NULL,
	[TransactionStatusId] [bigint] NULL,
	[TransactionDate] [datetime2](7) NULL,
	[TransactionId] [bigint] NULL,
	[UserId] [bigint] NULL,
	[ComputerId] [bigint] NULL,
	[ApplicationId] [bigint] NULL,
	[EventId] [bigint] NULL,
	[Comment] [nvarchar](max) NULL,
	[MetadataId] [bigint] NULL,
	[Data] [nvarchar](max) NULL,
	[DataUUID] [nvarchar](450) NULL,
	[DataPresentation] [nvarchar](max) NULL,
	[WorkServerId] [bigint] NULL,
	[PrimaryPortId] [bigint] NULL,
	[SecondaryPortId] [bigint] NULL,
 CONSTRAINT [PK_RowsData] PRIMARY KEY CLUSTERED 
(
	[InformationSystemId] ASC,
	[Period] ASC,
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[SecondaryPorts](
	[InformationSystemId] [bigint] NOT NULL,
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NULL,
 CONSTRAINT [PK_SecondaryPorts] PRIMARY KEY CLUSTERED 
(
	[InformationSystemId] ASC,
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Severities](
	[InformationSystemId] [bigint] NOT NULL,
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NULL,
	[Presentation] [nvarchar](500) NULL,
 CONSTRAINT [PK_Severities] PRIMARY KEY CLUSTERED 
(
	[InformationSystemId] ASC,
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Users](
	[InformationSystemId] [bigint] NOT NULL,
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NULL,
	[Uuid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[InformationSystemId] ASC,
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[WorkServers](
	[InformationSystemId] [bigint] NOT NULL,
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NULL,
 CONSTRAINT [PK_WorkServers] PRIMARY KEY CLUSTERED 
(
	[InformationSystemId] ASC,
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE VIEW [dbo].[vw_EventLog]
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
				                AND [RD].[SecondaryPortId] = [SPRT].[Id]
GO

CREATE TABLE [dbo].[LogFiles](
	[InformationSystemId] [bigint] NOT NULL,
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](max) NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModificationDate] [datetime2](7) NOT NULL,
	[LastEventNumber] [bigint] NOT NULL,
	[LastCurrentFileReferences] [nvarchar](max) NULL,
	[LastCurrentFileData] [nvarchar](max) NULL,
	[LastStreamPosition] [bigint] NULL,
 CONSTRAINT [PK_LogFiles] PRIMARY KEY CLUSTERED 
(
	[InformationSystemId] ASC,
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[TransactionStatuses](
	[InformationSystemId] [bigint] NOT NULL,
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NULL,
	[Presentation] [nvarchar](500) NULL,
 CONSTRAINT [PK_TransactionStatuses] PRIMARY KEY CLUSTERED 
(
	[InformationSystemId] ASC,
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Applications_InformationSystemId_Name] ON [dbo].[Applications]
(
	[InformationSystemId] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Computers_InformationSystemId_Name] ON [dbo].[Computers]
(
	[InformationSystemId] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Events_InformationSystemId_Name] ON [dbo].[Events]
(
	[InformationSystemId] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Metadata_InformationSystemId_Name_Uuid] ON [dbo].[Metadata]
(
	[InformationSystemId] ASC,
	[Name] ASC,
	[Uuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_PrimaryPorts_InformationSystemId_Name] ON [dbo].[PrimaryPorts]
(
	[InformationSystemId] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_RowsData_InformationSystemId_DataUUID] ON [dbo].[RowsData]
(
	[InformationSystemId] ASC,
	[DataUUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_RowsData_InformationSystemId_UserId_Period] ON [dbo].[RowsData]
(
	[InformationSystemId] ASC,
	[UserId] ASC,
	[Period] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_SecondaryPorts_InformationSystemId_Name] ON [dbo].[SecondaryPorts]
(
	[InformationSystemId] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Severities_InformationSystemId_Name] ON [dbo].[Severities]
(
	[InformationSystemId] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Users_InformationSystemId_Name_Uuid] ON [dbo].[Users]
(
	[InformationSystemId] ASC,
	[Name] ASC,
	[Uuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_WorkServers_InformationSystemId_Name] ON [dbo].[WorkServers]
(
	[InformationSystemId] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

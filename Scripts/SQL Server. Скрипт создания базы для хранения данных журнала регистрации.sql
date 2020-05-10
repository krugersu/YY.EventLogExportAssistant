IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Applications] (
    [InformationSystemId] bigint NOT NULL,
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(250) NULL,
    CONSTRAINT [PK_Applications] PRIMARY KEY ([InformationSystemId], [Id])
);

GO

CREATE TABLE [Computers] (
    [InformationSystemId] bigint NOT NULL,
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(250) NULL,
    CONSTRAINT [PK_Computers] PRIMARY KEY ([InformationSystemId], [Id])
);

GO

CREATE TABLE [Events] (
    [InformationSystemId] bigint NOT NULL,
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(250) NULL,
    CONSTRAINT [PK_Events] PRIMARY KEY ([InformationSystemId], [Id])
);

GO

CREATE TABLE [InformationSystems] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(250) NULL,
    [Description] nvarchar(500) NULL,
    CONSTRAINT [PK_InformationSystems] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [LogFiles] (
    [InformationSystemId] bigint NOT NULL,
    [FileName] nvarchar(450) NOT NULL,
    [CreateDate] datetime2 NOT NULL,
    [Id] bigint NOT NULL IDENTITY,
    [ModificationDate] datetime2 NOT NULL,
    [LastEventNumber] bigint NOT NULL,
    [LastCurrentFileReferences] nvarchar(max) NULL,
    [LastCurrentFileData] nvarchar(max) NULL,
    [LastStreamPosition] bigint NULL,
    CONSTRAINT [PK_LogFiles] PRIMARY KEY ([InformationSystemId], [FileName], [CreateDate], [Id])
);

GO

CREATE TABLE [Metadata] (
    [InformationSystemId] bigint NOT NULL,
    [Id] bigint NOT NULL IDENTITY,
    [Uuid] uniqueidentifier NOT NULL,
    [Name] nvarchar(250) NULL,
    CONSTRAINT [PK_Metadata] PRIMARY KEY ([InformationSystemId], [Id])
);

GO

CREATE TABLE [PrimaryPorts] (
    [InformationSystemId] bigint NOT NULL,
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(250) NULL,
    CONSTRAINT [PK_PrimaryPorts] PRIMARY KEY ([InformationSystemId], [Id])
);

GO

CREATE TABLE [RowsData] (
    [InformationSystemId] bigint NOT NULL,
    [Period] datetimeoffset NOT NULL,
    [Id] bigint NOT NULL,
    [SeverityId] bigint NULL,
    [ConnectId] bigint NULL,
    [Session] bigint NULL,
    [TransactionStatusId] bigint NULL,
    [TransactionDate] datetime2 NULL,
    [TransactionId] bigint NULL,
    [UserId] bigint NULL,
    [ComputerId] bigint NULL,
    [ApplicationId] bigint NULL,
    [EventId] bigint NULL,
    [Comment] nvarchar(max) NULL,
    [MetadataId] bigint NULL,
    [Data] nvarchar(max) NULL,
    [DataUUID] nvarchar(450) NULL,
    [DataPresentation] nvarchar(max) NULL,
    [WorkServerId] bigint NULL,
    [PrimaryPortId] bigint NULL,
    [SecondaryPortId] bigint NULL,
    CONSTRAINT [PK_RowsData] PRIMARY KEY ([InformationSystemId], [Period], [Id])
);

GO

CREATE TABLE [SecondaryPorts] (
    [InformationSystemId] bigint NOT NULL,
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(250) NULL,
    CONSTRAINT [PK_SecondaryPorts] PRIMARY KEY ([InformationSystemId], [Id])
);

GO

CREATE TABLE [Severities] (
    [InformationSystemId] bigint NOT NULL,
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(250) NULL,
    CONSTRAINT [PK_Severities] PRIMARY KEY ([InformationSystemId], [Id])
);

GO

CREATE TABLE [TransactionStatuses] (
    [InformationSystemId] bigint NOT NULL,
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(250) NULL,
    CONSTRAINT [PK_TransactionStatuses] PRIMARY KEY ([InformationSystemId], [Id])
);

GO

CREATE TABLE [Users] (
    [InformationSystemId] bigint NOT NULL,
    [Id] bigint NOT NULL IDENTITY,
    [Uuid] uniqueidentifier NOT NULL,
    [Name] nvarchar(250) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([InformationSystemId], [Id])
);

GO

CREATE TABLE [WorkServers] (
    [InformationSystemId] bigint NOT NULL,
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(250) NULL,
    CONSTRAINT [PK_WorkServers] PRIMARY KEY ([InformationSystemId], [Id])
);

GO

CREATE INDEX [IX_RowsData_InformationSystemId_DataUUID] ON [RowsData] ([InformationSystemId], [DataUUID]);

GO

CREATE INDEX [IX_RowsData_InformationSystemId_UserId_Period] ON [RowsData] ([InformationSystemId], [UserId], [Period]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200509202130_Initialization', N'3.1.3');

GO


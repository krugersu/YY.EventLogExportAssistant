create table if not exists applications
(
	InformationSystemId bigint not null,
	Id bigint auto_increment,
	Name varchar(500) null,
	primary key (Id, InformationSystemId),
	constraint IX_Applications_InformationSystemId_Id
		unique (InformationSystemId, Id)
);

create index IX_Applications_InformationSystemId_Name
	on applications (InformationSystemId, Name);

create table if not exists computers
(
	InformationSystemId bigint not null,
	Id bigint auto_increment,
	Name varchar(500) null,
	primary key (Id, InformationSystemId),
	constraint IX_Computers_InformationSystemId_Id
		unique (InformationSystemId, Id)
);

create index IX_Computers_InformationSystemId_Name
	on computers (InformationSystemId, Name);

create table if not exists events
(
	InformationSystemId bigint not null,
	Id bigint auto_increment,
	Name varchar(500) null,
	Presentation varchar(500) null,
	primary key (Id, InformationSystemId),
	constraint IX_Events_InformationSystemId_Id
		unique (InformationSystemId, Id)
);

create index IX_Events_InformationSystemId_Name
	on events (InformationSystemId, Name);

create table if not exists informationsystems
(
	Id bigint auto_increment,
	Name varchar(250) null,
	Description varchar(500) null,
	constraint IX_InformationSystems_Id
		unique (Id)
);

alter table informationsystems
	add primary key (Id);

create table if not exists logfiles
(
	InformationSystemId bigint not null,
	Id bigint auto_increment,
	FileName varchar(255) null,
	CreateDate datetime(6) not null,
	ModificationDate datetime(6) not null,
	LastEventNumber bigint not null,
	LastCurrentFileReferences longtext null,
	LastCurrentFileData longtext null,
	LastStreamPosition bigint null,
	primary key (Id, InformationSystemId),
	constraint IX_LogFiles_InformationSystemId_FileName_CreateDate_Id
		unique (InformationSystemId, FileName, CreateDate, Id)
);

create table if not exists metadata
(
	InformationSystemId bigint not null,
	Id bigint auto_increment,
	Name varchar(500) null,
	Uuid char(36) not null,
	primary key (Id, InformationSystemId),
	constraint IX_Metadata_InformationSystemId_Id
		unique (InformationSystemId, Id)
);

create index IX_Metadata_InformationSystemId_Name_Uuid
	on metadata (InformationSystemId, Name, Uuid);

create table if not exists primaryports
(
	InformationSystemId bigint not null,
	Id bigint auto_increment,
	Name varchar(500) null,
	primary key (Id, InformationSystemId),
	constraint IX_PrimaryPorts_InformationSystemId_Id
		unique (InformationSystemId, Id)
);

create index IX_PrimaryPorts_InformationSystemId_Name
	on primaryports (InformationSystemId, Name);

create table if not exists rowsdata
(
	InformationSystemId bigint not null,
	Id bigint not null,
	Period datetime(6) not null,
	SeverityId bigint null,
	ConnectId bigint null,
	Session bigint null,
	TransactionStatusId bigint null,
	TransactionDate datetime(6) null,
	TransactionId bigint null,
	UserId bigint null,
	ComputerId bigint null,
	ApplicationId bigint null,
	EventId bigint null,
	Comment longtext null,
	MetadataId bigint null,
	Data longtext null,
	DataUUID varchar(255) null,
	DataPresentation longtext null,
	WorkServerId bigint null,
	PrimaryPortId bigint null,
	SecondaryPortId bigint null,
	constraint IX_RowsData_InformationSystemId_Period_Id
		unique (InformationSystemId, Period, Id)
);

create index IX_RowsData_InformationSystemId_DataUUID
	on rowsdata (InformationSystemId, DataUUID);

create index IX_RowsData_InformationSystemId_UserId_Period
	on rowsdata (InformationSystemId, UserId, Period);

alter table rowsdata
	add primary key (InformationSystemId, Period, Id);

create table if not exists secondaryports
(
	InformationSystemId bigint not null,
	Id bigint auto_increment,
	Name varchar(500) null,
	primary key (Id, InformationSystemId),
	constraint IX_SecondaryPorts_InformationSystemId_Id
		unique (InformationSystemId, Id)
);

create index IX_SecondaryPorts_InformationSystemId_Name
	on secondaryports (InformationSystemId, Name);

create table if not exists severities
(
	InformationSystemId bigint not null,
	Id bigint auto_increment,
	Name varchar(500) null,
	primary key (Id, InformationSystemId),
	constraint IX_Severities_InformationSystemId_Id
		unique (InformationSystemId, Id)
);

create index IX_Severities_InformationSystemId_Name
	on severities (InformationSystemId, Name);

create table if not exists transactionstatuses
(
	InformationSystemId bigint not null,
	Id bigint auto_increment,
	Name varchar(500) null,
	primary key (Id, InformationSystemId)
);

create table if not exists users
(
	InformationSystemId bigint not null,
	Id bigint auto_increment,
	Name varchar(500) null,
	Uuid char(36) not null,
	primary key (Id, InformationSystemId),
	constraint IX_Users_InformationSystemId_Id
		unique (InformationSystemId, Id)
);

create index IX_Users_InformationSystemId_Name_Uuid
	on users (InformationSystemId, Name, Uuid);

create table if not exists workservers
(
	InformationSystemId bigint not null,
	Id bigint auto_increment,
	Name varchar(500) null,
	primary key (Id, InformationSystemId),
	constraint IX_WorkServers_InformationSystemId_Id
		unique (InformationSystemId, Id)
);

create index IX_WorkServers_InformationSystemId_Name
	on workservers (InformationSystemId, Name);

create or replace definer = YPermitin@`%` view vw_eventlog as
	select `rd`.`InformationSystemId` AS `InformationSystemId`,
       `infs`.`Name`              AS `InformationSystemName`,
       `rd`.`Period`              AS `Period`,
       `rd`.`Id`                  AS `RowId`,
       `rd`.`SeverityId`          AS `SeverityId`,
       `sv`.`Name`                AS `SeverityName`,
       `rd`.`ConnectId`           AS `Connection`,
       `rd`.`Session`             AS `Session`,
       `rd`.`TransactionStatusId` AS `TransactionStatusId`,
       `rd`.`TransactionDate`     AS `TransactionDate`,
       `rd`.`TransactionId`       AS `TransactionId`,
       `rd`.`UserId`              AS `UserId`,
       `usr`.`Name`               AS `UserName`,
       `usr`.`Uuid`               AS `UserUUID`,
       `rd`.`ComputerId`          AS `ComputerId`,
       `cmp`.`Name`               AS `ASComputerName`,
       `rd`.`Data`                AS `Data`,
       `rd`.`DataUUID`            AS `DataUUID`,
       `rd`.`DataPresentation`    AS `DataPresentation`,
       `rd`.`Comment`             AS `Comment`,
       `rd`.`ApplicationId`       AS `ApplicationId`,
       `apps`.`Name`              AS `ApplicationName`,
       `rd`.`EventId`             AS `EventId`,
       `evnt`.`Name`              AS `EventName`,
       `rd`.`MetadataId`          AS `MetadataId`,
       `meta`.`Name`              AS `MetadataName`,
       `meta`.`Uuid`              AS `MetadataUUID`,
       `rd`.`WorkServerId`        AS `WorkServerId`,
       `wsrv`.`Name`              AS `WorkServerName`,
       `rd`.`PrimaryPortId`       AS `PrimaryPortId`,
       `pprt`.`Name`              AS `PrimaryPortName`,
       `rd`.`SecondaryPortId`     AS `SecondaryPortId`,
       `sprt`.`Name`              AS `SecondaryPortName`
from ((((((((((`rowsdata` `rd` left join `informationsystems` `infs` on ((`rd`.`InformationSystemId` = `infs`.`Id`))) left join `severities` `sv` on ((
        (`rd`.`InformationSystemId` = `sv`.`InformationSystemId`) and
        (`rd`.`SeverityId` = `sv`.`Id`)))) left join `users` `usr` on ((
        (`rd`.`InformationSystemId` = `usr`.`InformationSystemId`) and
        (`rd`.`UserId` = `usr`.`Id`)))) left join `computers` `cmp` on ((
        (`rd`.`InformationSystemId` = `cmp`.`InformationSystemId`) and
        (`rd`.`ComputerId` = `cmp`.`Id`)))) left join `applications` `apps` on ((
        (`rd`.`InformationSystemId` = `apps`.`InformationSystemId`) and
        (`rd`.`ApplicationId` = `apps`.`Id`)))) left join `events` `evnt` on ((
        (`rd`.`InformationSystemId` = `evnt`.`InformationSystemId`) and
        (`rd`.`EventId` = `evnt`.`Id`)))) left join `metadata` `meta` on ((
        (`rd`.`InformationSystemId` = `meta`.`InformationSystemId`) and
        (`rd`.`MetadataId` = `meta`.`Id`)))) left join `workservers` `wsrv` on ((
        (`rd`.`InformationSystemId` = `wsrv`.`InformationSystemId`) and
        (`rd`.`WorkServerId` = `wsrv`.`Id`)))) left join `primaryports` `pprt` on ((
        (`rd`.`InformationSystemId` = `pprt`.`InformationSystemId`) and (`rd`.`PrimaryPortId` = `pprt`.`Id`))))
         left join `secondaryports` `sprt`
                   on (((`rd`.`InformationSystemId` = `sprt`.`InformationSystemId`) and
                        (`rd`.`SecondaryPortId` = `sprt`.`Id`))));


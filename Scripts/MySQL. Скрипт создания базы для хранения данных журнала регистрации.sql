create table applications
(
    InformationSystemId bigint       not null,
    Id                  bigint auto_increment,
    Name                varchar(250) null,
    primary key (Id, InformationSystemId)
);

create table computers
(
    InformationSystemId bigint       not null,
    Id                  bigint auto_increment,
    Name                varchar(250) null,
    primary key (Id, InformationSystemId)
);

create table events
(
    InformationSystemId bigint       not null,
    Id                  bigint auto_increment,
    Name                varchar(250) null,
    primary key (Id, InformationSystemId)
);

create table informationsystems
(
    Id          bigint auto_increment
        primary key,
    Name        varchar(250) null,
    Description varchar(500) null
);

create table logfiles
(
    InformationSystemId       bigint       not null,
    FileName                  varchar(255) not null,
    CreateDate                datetime(6)  not null,
    Id                        bigint auto_increment,
    ModificationDate          datetime(6)  not null,
    LastEventNumber           bigint       not null,
    LastCurrentFileReferences longtext     null,
    LastCurrentFileData       longtext     null,
    LastStreamPosition        bigint       null,
    primary key (Id, InformationSystemId, FileName, CreateDate)
);

create table metadata
(
    InformationSystemId bigint       not null,
    Id                  bigint auto_increment,
    Uuid                char(36)     not null,
    Name                varchar(250) null,
    primary key (Id, InformationSystemId)
);

create table primaryports
(
    InformationSystemId bigint       not null,
    Id                  bigint auto_increment,
    Name                varchar(250) null,
    primary key (Id, InformationSystemId)
);

create table rowsdata
(
    InformationSystemId bigint       not null,
    Period datetime (6) not null,
    Id                  bigint       not null,
    SeverityId          bigint       null,
    ConnectId           bigint       null,
    Session             bigint       null,
    TransactionStatusId bigint       null,
    TransactionDate     datetime(6)  null,
    TransactionId       bigint       null,
    UserId              bigint       null,
    ComputerId          bigint       null,
    ApplicationId       bigint       null,
    EventId             bigint       null,
    Comment             longtext     null,
    MetadataId          bigint       null,
    Data                longtext     null,
    DataUUID            varchar(255) null,
    DataPresentation    longtext     null,
    WorkServerId        bigint       null,
    PrimaryPortId       bigint       null,
    SecondaryPortId     bigint       null,
    primary key (InformationSystemId, Period, Id)
);

create index IX_RowsData_InformationSystemId_DataUUID
    on rowsdata (InformationSystemId, DataUUID);

create index IX_RowsData_InformationSystemId_UserId_Period
    on rowsdata (InformationSystemId, UserId, Period);

create table secondaryports
(
    InformationSystemId bigint       not null,
    Id                  bigint auto_increment,
    Name                varchar(250) null,
    primary key (Id, InformationSystemId)
);

create table severities
(
    InformationSystemId bigint       not null,
    Id                  bigint auto_increment,
    Name                varchar(250) null,
    primary key (Id, InformationSystemId)
);

create table transactionstatuses
(
    InformationSystemId bigint       not null,
    Id                  bigint auto_increment,
    Name                varchar(250) null,
    primary key (Id, InformationSystemId)
);

create table users
(
    InformationSystemId bigint       not null,
    Id                  bigint auto_increment,
    Uuid                char(36)     not null,
    Name                varchar(250) null,
    primary key (Id, InformationSystemId)
);

create table workservers
(
    InformationSystemId bigint       not null,
    Id                  bigint auto_increment,
    Name                varchar(250) null,
    primary key (Id, InformationSystemId)
);

create view vw_eventlog as
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
from ((((((((((`eventlogexporttest`.`rowsdata` `rd` left join `eventlogexporttest`.`informationsystems` `infs` on ((`rd`.`InformationSystemId` = `infs`.`Id`))) left join `eventlogexporttest`.`severities` `sv` on ((
        (`rd`.`InformationSystemId` = `sv`.`InformationSystemId`) and
        (`rd`.`SeverityId` = `sv`.`Id`)))) left join `eventlogexporttest`.`users` `usr` on ((
        (`rd`.`InformationSystemId` = `usr`.`InformationSystemId`) and
        (`rd`.`UserId` = `usr`.`Id`)))) left join `eventlogexporttest`.`computers` `cmp` on ((
        (`rd`.`InformationSystemId` = `cmp`.`InformationSystemId`) and
        (`rd`.`ComputerId` = `cmp`.`Id`)))) left join `eventlogexporttest`.`applications` `apps` on ((
        (`rd`.`InformationSystemId` = `apps`.`InformationSystemId`) and
        (`rd`.`ApplicationId` = `apps`.`Id`)))) left join `eventlogexporttest`.`events` `evnt` on ((
        (`rd`.`InformationSystemId` = `evnt`.`InformationSystemId`) and
        (`rd`.`EventId` = `evnt`.`Id`)))) left join `eventlogexporttest`.`metadata` `meta` on ((
        (`rd`.`InformationSystemId` = `meta`.`InformationSystemId`) and
        (`rd`.`MetadataId` = `meta`.`Id`)))) left join `eventlogexporttest`.`workservers` `wsrv` on ((
        (`rd`.`InformationSystemId` = `wsrv`.`InformationSystemId`) and
        (`rd`.`WorkServerId` = `wsrv`.`Id`)))) left join `eventlogexporttest`.`primaryports` `pprt` on ((
        (`rd`.`InformationSystemId` = `pprt`.`InformationSystemId`) and (`rd`.`PrimaryPortId` = `pprt`.`Id`))))
         left join `eventlogexporttest`.`secondaryports` `sprt`
                   on (((`rd`.`InformationSystemId` = `sprt`.`InformationSystemId`) and
                        (`rd`.`SecondaryPortId` = `sprt`.`Id`))));


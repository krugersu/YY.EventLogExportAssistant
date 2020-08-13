-- Скрипт состоит из трех этапов
--  1 - Пересоздание таблиц ссылочных данных
--  2 - Добавление новых индексов по имени ссылочных сущностей
--  3 - Заполнение полей Presentation
--  4 - Заполнение полей DataUUID (ранее для LGD-формата они могли быть не заполнены из-за ошибки)
-- ВНИМАНИЕ!!! Может потребовать адаптации под спицифику данных.

-- 1. Все поля Name у ссылочных сущностей увеличены до 500 (старое значение 250)
-- Также добавлены поля Presentation для некоторых сущностей
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_Applications
	(
	InformationSystemId bigint NOT NULL,
	Id bigint NOT NULL IDENTITY (1, 1),
	Name nvarchar(500) NULL,
	Presentation nvarchar(500) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Applications SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_Applications ON
GO
IF EXISTS(SELECT * FROM dbo.Applications)
	 EXEC('INSERT INTO dbo.Tmp_Applications (InformationSystemId, Id, Name, Presentation)
		SELECT InformationSystemId, Id, Name, Presentation FROM dbo.Applications WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Applications OFF
GO
DROP TABLE dbo.Applications
GO
EXECUTE sp_rename N'dbo.Tmp_Applications', N'Applications', 'OBJECT' 
GO
ALTER TABLE dbo.Applications ADD CONSTRAINT
	PK_Applications PRIMARY KEY CLUSTERED 
	(
	InformationSystemId,
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT

COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_Computers
	(
	InformationSystemId bigint NOT NULL,
	Id bigint NOT NULL IDENTITY (1, 1),
	Name nvarchar(500) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Computers SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_Computers ON
GO
IF EXISTS(SELECT * FROM dbo.Computers)
	 EXEC('INSERT INTO dbo.Tmp_Computers (InformationSystemId, Id, Name)
		SELECT InformationSystemId, Id, Name FROM dbo.Computers WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Computers OFF
GO
DROP TABLE dbo.Computers
GO
EXECUTE sp_rename N'dbo.Tmp_Computers', N'Computers', 'OBJECT' 
GO
ALTER TABLE dbo.Computers ADD CONSTRAINT
	PK_Computers PRIMARY KEY CLUSTERED 
	(
	InformationSystemId,
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT

BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_Events
	(
	InformationSystemId bigint NOT NULL,
	Id bigint NOT NULL IDENTITY (1, 1),
	Name nvarchar(500) NULL,
	Presentation nvarchar(500) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Events SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_Events ON
GO
IF EXISTS(SELECT * FROM dbo.Events)
	 EXEC('INSERT INTO dbo.Tmp_Events (InformationSystemId, Id, Name, Presentation)
		SELECT InformationSystemId, Id, Name, Presentation FROM dbo.Events WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Events OFF
GO
DROP TABLE dbo.Events
GO
EXECUTE sp_rename N'dbo.Tmp_Events', N'Events', 'OBJECT' 
GO
ALTER TABLE dbo.Events ADD CONSTRAINT
	PK_Events PRIMARY KEY CLUSTERED 
	(
	InformationSystemId,
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT

BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_Metadata
	(
	InformationSystemId bigint NOT NULL,
	Id bigint NOT NULL IDENTITY (1, 1),
	Name nvarchar(500) NULL,
	Uuid uniqueidentifier NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Metadata SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_Metadata ON
GO
IF EXISTS(SELECT * FROM dbo.Metadata)
	 EXEC('INSERT INTO dbo.Tmp_Metadata (InformationSystemId, Id, Name, Uuid)
		SELECT InformationSystemId, Id, Name, Uuid FROM dbo.Metadata WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Metadata OFF
GO
DROP TABLE dbo.Metadata
GO
EXECUTE sp_rename N'dbo.Tmp_Metadata', N'Metadata', 'OBJECT' 
GO
ALTER TABLE dbo.Metadata ADD CONSTRAINT
	PK_Metadata PRIMARY KEY CLUSTERED 
	(
	InformationSystemId,
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT

BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_PrimaryPorts
	(
	InformationSystemId bigint NOT NULL,
	Id bigint NOT NULL IDENTITY (1, 1),
	Name nvarchar(500) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_PrimaryPorts SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_PrimaryPorts ON
GO
IF EXISTS(SELECT * FROM dbo.PrimaryPorts)
	 EXEC('INSERT INTO dbo.Tmp_PrimaryPorts (InformationSystemId, Id, Name)
		SELECT InformationSystemId, Id, Name FROM dbo.PrimaryPorts WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_PrimaryPorts OFF
GO
DROP TABLE dbo.PrimaryPorts
GO
EXECUTE sp_rename N'dbo.Tmp_PrimaryPorts', N'PrimaryPorts', 'OBJECT' 
GO
ALTER TABLE dbo.PrimaryPorts ADD CONSTRAINT
	PK_PrimaryPorts PRIMARY KEY CLUSTERED 
	(
	InformationSystemId,
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT

BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_SecondaryPorts
	(
	InformationSystemId bigint NOT NULL,
	Id bigint NOT NULL IDENTITY (1, 1),
	Name nvarchar(500) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_SecondaryPorts SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_SecondaryPorts ON
GO
IF EXISTS(SELECT * FROM dbo.SecondaryPorts)
	 EXEC('INSERT INTO dbo.Tmp_SecondaryPorts (InformationSystemId, Id, Name)
		SELECT InformationSystemId, Id, Name FROM dbo.SecondaryPorts WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_SecondaryPorts OFF
GO
DROP TABLE dbo.SecondaryPorts
GO
EXECUTE sp_rename N'dbo.Tmp_SecondaryPorts', N'SecondaryPorts', 'OBJECT' 
GO
ALTER TABLE dbo.SecondaryPorts ADD CONSTRAINT
	PK_SecondaryPorts PRIMARY KEY CLUSTERED 
	(
	InformationSystemId,
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT

BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_Severities
	(
	InformationSystemId bigint NOT NULL,
	Id bigint NOT NULL IDENTITY (1, 1),
	Name nvarchar(500) NULL,
	Presentation nvarchar(500) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Severities SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_Severities ON
GO
IF EXISTS(SELECT * FROM dbo.Severities)
	 EXEC('INSERT INTO dbo.Tmp_Severities (InformationSystemId, Id, Name, Presentation)
		SELECT InformationSystemId, Id, Name, Presentation FROM dbo.Severities WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Severities OFF
GO
DROP TABLE dbo.Severities
GO
EXECUTE sp_rename N'dbo.Tmp_Severities', N'Severities', 'OBJECT' 
GO
ALTER TABLE dbo.Severities ADD CONSTRAINT
	PK_Severities PRIMARY KEY CLUSTERED 
	(
	InformationSystemId,
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT

BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_TransactionStatuses
	(
	InformationSystemId bigint NOT NULL,
	Id bigint NOT NULL IDENTITY (1, 1),
	Name nvarchar(500) NULL,
	Presentation nvarchar(500) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_TransactionStatuses SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_TransactionStatuses ON
GO
IF EXISTS(SELECT * FROM dbo.TransactionStatuses)
	 EXEC('INSERT INTO dbo.Tmp_TransactionStatuses (InformationSystemId, Id, Name, Presentation)
		SELECT InformationSystemId, Id, Name, Presentation FROM dbo.TransactionStatuses WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_TransactionStatuses OFF
GO
DROP TABLE dbo.TransactionStatuses
GO
EXECUTE sp_rename N'dbo.Tmp_TransactionStatuses', N'TransactionStatuses', 'OBJECT' 
GO
ALTER TABLE dbo.TransactionStatuses ADD CONSTRAINT
	PK_TransactionStatuses PRIMARY KEY CLUSTERED 
	(
	InformationSystemId,
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT

BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_Users
	(
	InformationSystemId bigint NOT NULL,
	Id bigint NOT NULL IDENTITY (1, 1),
	Name nvarchar(501) NULL,
	Uuid uniqueidentifier NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Users SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_Users ON
GO
IF EXISTS(SELECT * FROM dbo.Users)
	 EXEC('INSERT INTO dbo.Tmp_Users (InformationSystemId, Id, Name, Uuid)
		SELECT InformationSystemId, Id, Name, Uuid FROM dbo.Users WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Users OFF
GO
DROP TABLE dbo.Users
GO
EXECUTE sp_rename N'dbo.Tmp_Users', N'Users', 'OBJECT' 
GO
ALTER TABLE dbo.Users ADD CONSTRAINT
	PK_Users PRIMARY KEY CLUSTERED 
	(
	InformationSystemId,
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT

BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_WorkServers
	(
	InformationSystemId bigint NOT NULL,
	Id bigint NOT NULL IDENTITY (1, 1),
	Name nvarchar(501) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_WorkServers SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_WorkServers ON
GO
IF EXISTS(SELECT * FROM dbo.WorkServers)
	 EXEC('INSERT INTO dbo.Tmp_WorkServers (InformationSystemId, Id, Name)
		SELECT InformationSystemId, Id, Name FROM dbo.WorkServers WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_WorkServers OFF
GO
DROP TABLE dbo.WorkServers
GO
EXECUTE sp_rename N'dbo.Tmp_WorkServers', N'WorkServers', 'OBJECT' 
GO
ALTER TABLE dbo.WorkServers ADD CONSTRAINT
	PK_WorkServers PRIMARY KEY CLUSTERED 
	(
	InformationSystemId,
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT

-- 2. Добавлены индексы по полям идентификатора информационной системы + имя значения ссылки
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

-- 3. Заполнение полей Presentation
UPDATE [dbo].[Applications]
SET [Presentation] = 
	CASE 
		WHEN [Name] = '1CV8' THEN 'Толстый клиент'
        WHEN [Name] = '1CV8C' THEN 'Тонкий клиент'
        WHEN [Name] = 'WebClient' THEN 'Веб-клиент'
        WHEN [Name] = 'Designer' THEN 'Конфигуратор'
        WHEN [Name] = 'COMConnection' THEN 'Внешнее соединение (COM, обычное)'
        WHEN [Name] = 'WSConnection' THEN 'Сессия web-сервиса'
        WHEN [Name] = 'BackgroundJob' THEN 'Фоновое задание'
        WHEN [Name] = 'SystemBackgroundJob' THEN 'Системное фоновое задание'
        WHEN [Name] = 'SrvrConsole' THEN 'Консоль кластера'
        WHEN [Name] = 'COMConsole' THEN 'Внешнее соединение (COM, административное)'
        WHEN [Name] = 'JobScheduler' THEN 'Планировщик заданий'
        WHEN [Name] = 'Debugger' THEN 'Отладчик'
        WHEN [Name] = 'RAS' THEN 'Сервер администрирования'
		ELSE [Name]
	END

UPDATE [dbo].[Severities]
SET [Presentation] = 
	CASE 
		WHEN [Name] = 'Unknown' THEN 'Неизвестно'
        WHEN [Name] = 'Error' THEN 'Ошибка'
        WHEN [Name] = 'Information' THEN 'Информация'
        WHEN [Name] = 'Note' THEN 'Примечание'
        WHEN [Name] = 'Warning' THEN 'Предупреждение'        
		ELSE [Name]
	END

UPDATE [dbo].[TransactionStatuses]
SET [Presentation] = 
	CASE 
		WHEN [Name] = 'Committed' THEN 'Зафиксирована'
        WHEN [Name] = 'Unfinished' THEN 'Не завершена'
        WHEN [Name] = 'NotApplicable' THEN 'Нет транзакции'
        WHEN [Name] = 'RolledBack' THEN 'Отменена'
        WHEN [Name] = 'Unknown' THEN 'Неизвестно'        
		ELSE [Name]
	END

UPDATE [dbo].[Events]
SET [Presentation] = 
	CASE 
		WHEN [Name] = '_$Access$_.Access' THEN 'Доступ.Доступ'
        WHEN [Name] = '_$Access$_.AccessDenied' THEN 'Доступ.Отказ в доступе'
        WHEN [Name] = '_$Data$_.Delete' THEN 'Данные.Удаление'
        WHEN [Name] = '_$Data$_.DeletePredefinedData' THEN ' Данные.Удаление предопределенных данных'
        WHEN [Name] = '_$Data$_.DeleteVersions' THEN 'Данные.Удаление версий'
        WHEN [Name] = '_$Data$_.New' THEN 'Данные.Добавление'
        WHEN [Name] = '_$Data$_.NewPredefinedData' THEN 'Данные.Добавление предопределенных данных'
        WHEN [Name] = '_$Data$_.NewVersion' THEN 'Данные.Добавление версиип'
        WHEN [Name] = '_$Data$_.Pos' THEN 'Данные.Проведение'
        WHEN [Name] = '_$Data$_.PredefinedDataInitialization' THEN 'Данные.Инициализация предопределенных данных'
        WHEN [Name] = '_$Data$_.PredefinedDataInitializationDataNotFound' THEN 'Данные.Инициализация предопределенных данных.Данные не найдены'
        WHEN [Name] = '_$Data$_.SetPredefinedDataInitialization' THEN 'Данные.Установка инициализации предопределенных данных'
        WHEN [Name] = '_$Data$_.SetStandardODataInterfaceContent' THEN 'Данные.Изменение состава стандартного интерфейса OData'
        WHEN [Name] = '_$Data$_.TotalsMaxPeriodUpdate' THEN 'Данные.Изменение максимального периода рассчитанных итогов'
        WHEN [Name] = '_$Data$_.TotalsMinPeriodUpdate' THEN 'Данные.Изменение минимального периода рассчитанных итогов'
        WHEN [Name] = '_$Data$_.Post' THEN 'Данные.Проведение'
        WHEN [Name] = '_$Data$_.Unpost' THEN 'Данные.Отмена проведения'
        WHEN [Name] = '_$Data$_.Update' THEN 'Данные.Изменение'
        WHEN [Name] = '_$Data$_.UpdatePredefinedData' THEN 'Данные.Изменение предопределенных данных'
        WHEN [Name] = '_$Data$_.VersionCommentUpdate' THEN 'Данные.Изменение комментария версии'
        WHEN [Name] = '_$InfoBase$_.ConfigExtensionUpdate' THEN 'Информационная база.Изменение расширения конфигурации'
        WHEN [Name] = '_$InfoBase$_.ConfigUpdate' THEN 'Информационная база.Изменение конфигурации'
        WHEN [Name] = '_$InfoBase$_.DBConfigBackgroundUpdateCancel' THEN 'Информационная база.Отмена фонового обновления'
        WHEN [Name] = '_$InfoBase$_.DBConfigBackgroundUpdateFinish' THEN 'Информационная база.Завершение фонового обновления'
        WHEN [Name] = '_$InfoBase$_.DBConfigBackgroundUpdateResume' THEN 'Информационная база.Продолжение (после приостановки) процесса фонового обновления'
        WHEN [Name] = '_$InfoBase$_.DBConfigBackgroundUpdateStart' THEN 'Информационная база.Запуск фонового обновления'
        WHEN [Name] = '_$InfoBase$_.DBConfigBackgroundUpdateSuspend' THEN 'Информационная база.Приостановка (пауза) процесса фонового обновления'
        WHEN [Name] = '_$InfoBase$_.DBConfigExtensionUpdate' THEN 'Информационная база.Изменение расширения конфигурации'
        WHEN [Name] = '_$InfoBase$_.DBConfigExtensionUpdateError' THEN 'Информационная база.Ошибка изменения расширения конфигурации'
        WHEN [Name] = '_$InfoBase$_.DBConfigUpdate' THEN 'Информационная база.Изменение конфигурации базы данных'
        WHEN [Name] = '_$InfoBase$_.DBConfigUpdateStart' THEN 'Информационная база.Запуск обновления конфигурации базы данных'
        WHEN [Name] = '_$InfoBase$_.DumpError' THEN 'Информационная база.Ошибка выгрузки в файл'
        WHEN [Name] = '_$InfoBase$_.DumpFinish' THEN 'Информационная база.Окончание выгрузки в файл'
        WHEN [Name] = '_$InfoBase$_.DumpStart' THEN 'Информационная база.Начало выгрузки в файл'
        WHEN [Name] = '_$InfoBase$_.EraseData' THEN ' Информационная база.Удаление данных информационной баз'
        WHEN [Name] = '_$InfoBase$_.EventLogReduce' THEN 'Информационная база.Сокращение журнала регистрации'
        WHEN [Name] = '_$InfoBase$_.EventLogReduceError' THEN 'Информационная база.Ошибка сокращения журнала регистрации'
        WHEN [Name] = '_$InfoBase$_.EventLogSettingsUpdate' THEN 'Информационная база.Изменение параметров журнала регистрации'
        WHEN [Name] = '_$InfoBase$_.EventLogSettingsUpdateError' THEN 'Информационная база.Ошибка при изменение настроек журнала регистрации'
        WHEN [Name] = '_$InfoBase$_.InfoBaseAdmParamsUpdate' THEN 'Информационная база.Изменение параметров информационной базы'
        WHEN [Name] = '_$InfoBase$_.InfoBaseAdmParamsUpdateError' THEN 'Информационная база.Ошибка изменения параметров информационной базы'
        WHEN [Name] = '_$InfoBase$_.IntegrationServiceActiveUpdate' THEN 'Информационная база.Изменение активности сервиса интеграции'
        WHEN [Name] = '_$InfoBase$_.IntegrationServiceSettingsUpdate' THEN 'Информационная база.Изменение настроек сервиса интеграции'
        WHEN [Name] = '_$InfoBase$_.MasterNodeUpdate' THEN 'Информационная база.Изменение главного узла'
        WHEN [Name] = '_$InfoBase$_.PredefinedDataUpdate' THEN 'Информационная база.Обновление предопределенных данных'
        WHEN [Name] = '_$InfoBase$_.RegionalSettingsUpdate' THEN 'Информационная база.Изменение региональных установок'
        WHEN [Name] = '_$InfoBase$_.RestoreError' THEN 'Информационная база.Ошибка загрузки из файла'
        WHEN [Name] = '_$InfoBase$_.RestoreFinish' THEN 'Информационная база.Окончание загрузки из файла'
        WHEN [Name] = '_$InfoBase$_.RestoreStart' THEN 'Информационная база.Начало загрузки из файла'
        WHEN [Name] = '_$InfoBase$_.SecondFactorAuthTemplateDelete' THEN 'Информационная база.Удаление шаблона вторго фактора аутентификации'
        WHEN [Name] = '_$InfoBase$_.SecondFactorAuthTemplateNew' THEN 'Информационная база.Добавление шаблона вторго фактора аутентификации'
        WHEN [Name] = '_$InfoBase$_.SecondFactorAuthTemplateUpdate' THEN 'Информационная база.Изменение шаблона вторго фактора аутентификации'
        WHEN [Name] = '_$InfoBase$_.SetPredefinedDataUpdate' THEN 'Информационная база.Установить обновление предопределенных данных'
        WHEN [Name] = '_$InfoBase$_.TARImportant' THEN 'Тестирование и исправление.Ошибка'
        WHEN [Name] = '_$InfoBase$_.TARInfo' THEN 'Тестирование и исправление.Сообщение'
        WHEN [Name] = '_$InfoBase$_.TARMess' THEN 'Тестирование и исправление.Предупреждение'
        WHEN [Name] = '_$Job$_.Cancel' THEN 'Фоновое задание.Отмена'
        WHEN [Name] = '_$Job$_.Fail' THEN 'Фоновое задание.Ошибка выполнения'
        WHEN [Name] = '_$Job$_.Start' THEN 'Фоновое задание.Запуск'
        WHEN [Name] = '_$Job$_.Succeed' THEN 'Фоновое задание.Успешное завершение'
        WHEN [Name] = '_$Job$_.Terminate' THEN 'Фоновое задание.Принудительное завершение'
        WHEN [Name] = '_$OpenIDProvider$_.NegativeAssertion' THEN 'Провайдер OpenID.Отклонено'
        WHEN [Name] = '_$OpenIDProvider$_.PositiveAssertion' THEN 'Провайдер OpenID.Подтверждено'
        WHEN [Name] = '_$PerformError$_' THEN 'Ошибка выполнения'
        WHEN [Name] = '_$Session$_.Authentication' THEN 'Сеанс.Аутентификация'
        WHEN [Name] = '_$Session$_.AuthenticationError' THEN 'Сеанс.Ошибка аутентификации'
        WHEN [Name] = '_$Session$_.AuthenticationFirstFactor' THEN 'Сеанс.Аутентификация первый фактор'
        WHEN [Name] = '_$Session$_.ConfigExtensionApplyError' THEN 'Сеанс.Ошибка применения расширения конфигурации'
        WHEN [Name] = '_$Session$_.Finish' THEN 'Сеанс.Завершение'
        WHEN [Name] = '_$Session$_.Start' THEN 'Сеанс.Начало'
        WHEN [Name] = '_$Transaction$_.Begin' THEN 'Транзакция.Начало'
        WHEN [Name] = '_$Transaction$_.Commit' THEN 'Транзакция.Фиксация'
        WHEN [Name] = '_$Transaction$_.Rollback' THEN 'Транзакция.Отмена'
        WHEN [Name] = '_$User$_.AuthenticationLock' THEN 'Пользователи.Блокировка аутентификации'
        WHEN [Name] = '_$User$_.AuthenticationUnlock' THEN 'Пользователи.Разблокировка аутентификации'
        WHEN [Name] = '_$User$_.AuthenticationUnlockError ' THEN 'Пользователи.Ошибка разблокировки аутентификации'
        WHEN [Name] = '_$User$_.Delete' THEN 'Пользователи.Удаление'
        WHEN [Name] = '_$User$_.DeleteError' THEN 'Пользователи.Ошибка удаления'
        WHEN [Name] = '_$User$_.New' THEN 'Пользователи.Добавление'
        WHEN [Name] = '_$User$_.NewError' THEN 'Пользователи.Ошибка добавления'
        WHEN [Name] = '_$User$_.Update' THEN 'Пользователи.Изменение'
        WHEN [Name] = '_$User$_.UpdateError' THEN 'Пользователи. Ошибка изменения'       
		ELSE [Name]
	END

-- 4. Заполнение поля DataUUID
UPDATE [dbo].[RowsData]
SET [DataUUID] = SUBSTRING([Data], CHARINDEX (':', [Data]) + 1, 32)
WHERE [Data] <> ''
	AND [Data] LIKE '{%,%:%}' 
	AND DataUUID IS NULL
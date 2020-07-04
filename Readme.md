# Помощник экспорта журнала регистрации

| Nuget-пакет | Актуальная версия | Описание |
| ----------- | ----------------- | -------- |
| YY.EventLogExportAssistant.Core | [![NuGet version](https://badge.fury.io/nu/YY.EventLogExportAssistant.Core.svg)](https://badge.fury.io/nu/YY.EventLogExportAssistant.Core) | Базовый пакет |
| YY.EventLogExportAssistant.SQLServer | [![NuGet version](https://badge.fury.io/nu/YY.EventLogExportAssistant.SQLServer.svg)](https://badge.fury.io/nu/YY.EventLogExportAssistant.SQLServer) | Пакет для экспорта в базу SQL Server |
| YY.EventLogExportAssistant.PostgreSQL | [![NuGet version](https://badge.fury.io/nu/YY.EventLogExportAssistant.PostgreSQL.svg)](https://badge.fury.io/nu/YY.EventLogExportAssistant.PostgreSQL) | Пакет для экспорта в базу PostgreSQL |
| YY.EventLogExportAssistant.ElasticSearch | [![NuGet version](https://badge.fury.io/nu/YY.EventLogExportAssistant.ElasticSearch.svg)](https://badge.fury.io/nu/YY.EventLogExportAssistant.ElasticSearch) | Пакет для экспорта в индексы ElasticSearch |

Решение для экспорта данных журнала регистрации платформы 1С:Предприятие 8.x в нестандартные хранилища данных.
С помощью библиотеки **[YY.EventLogReaderAssistant](https://github.com/YPermitin/YY.EventLogReaderAssistant)** реализовано чтение данных журнала регистрации как текстового формата (*.lgf, *.lgp), так и нового формата в виде SQLite-базы (*.lgd).

### Состояние сборки
| Windows |  Linux |
|:-------:|:------:|
| [![Build status](https://ci.appveyor.com/api/projects/status/lm4hex3gooyvaes2?svg=true)](https://ci.appveyor.com/project/YPermitin/yy-eventlogexportassistant) | [![Build Status](https://travis-ci.org/YPermitin/YY.EventLogExportAssistant.svg?branch=master)](https://travis-ci.org/YPermitin/YY.EventLogExportAssistant) |

## Состав репозитория

* Библиотеки
  * YY.EventLogExportAssistant.Core - ядро библиотеки с овновным функционалом чтения и передачи данных.
  * YY.EventLogExportAssistant.SQLServer - функционал для экспорта данных в базу SQL Server.
  * YY.EventLogExportAssistant.PostgreSQL - функционал для экспорта данных в базу PostgreSQL.
* Примеры приложений
  * YY.EventLogExportToSQLServer - пример приложения для экспорта данных в базу SQL Server.
  * YY.EventLogExportToPostgreSQL - пример приложения для экспорта данных в базу PostgreSQL.

## Требования и совместимость

Работа библиотеки тестировалась со следующими версиями компонентов:

* Платформа 1С:Предприятие версии от 8.3.6 и выше.
* SQL Server 2012 и более новые.
* PostgreSQL 9.6 и выше.

В большинстве случаев работоспособность подтверждается и на более старых версиях ПО, но меньше тестируется. Основная разработка ведется для Microsoft Windows, но некоторый функционал проверялся под *.nix.*

## Пример использования

Репозиторий содержит два примера консольных приложений для экспорта данных:

* YY.EventLogExportToSQLServer
* YY.EventLogExportToPostgreSQL

Для удобства приведем небольшой пример для выгрузки данных журнала регистрации в базу SQL Server.

### Конфигурация

Первое, с чего следует начать - это конфигурационный файл приложения "appsettings.json". Это JSON-файл со строкой подключения к базе данных, сведениями об информационной системе и параметрами обработки журнала регистрации. Располагается в корне каталога приложения.

```json
{
  "ConnectionStrings": {
    "EventLogDatabase": "Host=localhost;Port=5432;Database=EventLog;Username=YourUser;Password=YourPassword"
  },
  "InformationSystem": {
    "Name": "Бухгалтерия предприяния 3.0 (рабочая база)",
    "Description": "Журнал регистрации основной рабочей базы БУ 3.0"
  },
  "EventLog": {
    "SourcePath": "C:\\Program Files\\1cv8\\srvinfo\\reg_1541\\3f54d9a8-5457-41ad-9b43-10207c36f144\\1Cv8Log",
    "UseWatchMode": true,
    "WatchPeriod": 5,
    "Portion": 10000
  }
}
```

Секция **"ConnectionStrings"** содержит строку подключения **"EventLogDatabase"** к базе данных для экспорта. База будет создана автоматически при первом запуске приложения. Также можно создать ее вручную, главное чтобы структура была соответствующей. Имя строки подключения **"EventLogDatabase"** - это значение по умолчанию. Контекст приложения будет использовать ее автоматически, если это не переопределено в разработчиком явно.

Секция **"InformationSystem"** содержит название информационной системы и ее описание. Информационная система позволяет разделять хранение журналов регистрации разных баз 1С в одной базе данных.

Секция **"EventLog"** содержит параметры обработки журнала регистрации:

* **SourcePath** - путь к каталогу с файлами журнала регистрации. Может быть указан только каталог или конкретный файл журнала (1Cv8.lgf или 1Cv8.lgd).
* **UseWatchMode** - при значении false приложение завершит свою работу после загрузки всех данных. При значении true будет отслеживать появления новых данных пока приложение не будет явно закрыто.
* **WatchPeriod** - период в секундах, с которым приложение будет проверять наличие изменений. Используется, если параметр "UseWatchMode" установлен в true.
* **Portion** - количество записей, передаваемых в одной порции в базу данных из журнала регистрации.

Настройки "UseWatchMode" и "WatchPeriod" не относятся к библиотеке. Эти параметры добавлены лишь для примеров консольных приложений и используются в них же.

### Пример использования

На следующем листинге показан пример использования библиотеки.

```csharp
#region Private Static Member Variables

private static long _totalRows = 0;
private static long _lastPortionRows = 0;
private static DateTime _beginPortionExport;
private static DateTime _endPortionExport;

#endregion

#region Static Methods

static void Main()
{
    // 1. Инициализация настроек из файла "appsettings.json"
    IConfiguration Configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();

    IConfigurationSection eventLogSection = Configuration.GetSection("EventLog");
    string eventLogPath = eventLogSection.GetValue("SourcePath", string.Empty);
    int watchPeriodSeconds = eventLogSection.GetValue("WatchPeriod", 60);
    int watchPeriodSecondsMs = watchPeriodSeconds * 1000;
    bool useWatchMode = eventLogSection.GetValue("UseWatchMode", false);
    int portion = eventLogSection.GetValue("Portion", 1000);

    IConfigurationSection inforamtionSystemSection = Configuration.GetSection("InformationSystem");
    string inforamtionSystemName = inforamtionSystemSection.GetValue("Name", string.Empty);
    string inforamtionSystemDescription = inforamtionSystemSection.GetValue("Description", string.Empty);

    if (string.IsNullOrEmpty(eventLogPath))
    {
        Console.WriteLine("Не указан каталог с файлами данных журнала регистрации.");
        Console.WriteLine("Для выхода нажмите любую клавишу...");
        Console.Read();
        return;
    }

    Console.WriteLine();
    Console.WriteLine();

    // 2. (опционально) Инициализация настроек подключения к базе данных для экспорта
    string connectionString = Configuration.GetConnectionString("EventLogDatabase");
    DbContextOptions<EventLogContext> options = new DbContextOptions<EventLogContext>();
    var optionsBuilder = new DbContextOptionsBuilder<EventLogContext>();
    optionsBuilder.UseSqlServer(connectionString);

    // 3. Создаем объект для экспорта данных
    EventLogExportMaster exporter = new EventLogExportMaster();
    // 3.1. Устанавливаем каталог с файлами журнала регистрации
    exporter.SetEventLogPath(eventLogPath);

    // 3.2. Инициализируем назначение экспорта данных. Для каждого назначения - свой класс, наследуемый от класса
    // "EventLogOnTarget" и устанавливаем в нем информационную систему для выгрузки.
    // Для SQL Server - "EventLogOnSQLServer"
    // Для PostgreSQL - "EventLogOnPostgreSQL"
    // Можно создать собственный класс для выгрузки в произвольное хранилище.
    EventLogOnSQLServer target = new EventLogOnSQLServer(optionsBuilder.Options, portion);
    target.SetInformationSystem(new InformationSystemsBase()
    {
        Name = inforamtionSystemName,
        Description = inforamtionSystemDescription
    });

    // 4. Устанавливаем назначение экспорта
    exporter.SetTarget(target);

    // 5. Подписываемся на события экспорта данных
    // 5.1. Событие "Перед отправкой данных"
    exporter.BeforeExportData += BeforeExportData;
    // 5.2. Событие "После отправки данных"
    exporter.AfterExportData += AfterExportData;         

    // 6. Выгрузка данных
    if (useWatchMode)
    {
        // При настройке "WatchMode" = true выгружаем все накопившиеся данные,
        // а после проверяем новые данные для выгрузки кажде N секунд из настройки "WatchPeriod"
        while (true)
        {
            if (Console.KeyAvailable)
                if (Console.ReadKey().KeyChar == 'q')
                    break;

            while (exporter.NewDataAvailiable())
            {
                exporter.SendData();
                Thread.Sleep(watchPeriodSecondsMs);
            }                    
        }
    } else // При настройке "WatchMode" = false просто выгружаем все накопившиеся данные
        while (exporter.NewDataAvailiable())
            exporter.SendData();

    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("Для выхода нажмите любую клавишу...");
    Console.Read();
}

#endregion
```

Так выглядят примеры обработчиков событий "Перед экспортом данных" и "После экспорта данных".

```csharp
#region Events

private static void BeforeExportData(BeforeExportDataEventArgs e)
{
    _beginPortionExport = DateTime.Now;
    _lastPortionRows = e.Rows.Count;
    _totalRows += e.Rows.Count;

    Console.SetCursorPosition(0, 0);
    Console.WriteLine("[{0}] Last read: {1}             ", DateTime.Now, e.Rows.Count);
}
private static void AfterExportData(AfterExportDataEventArgs e)
{
    _endPortionExport = DateTime.Now;
    var duration = _endPortionExport - _beginPortionExport;

    Console.WriteLine("[{0}] Total read: {1}            ", DateTime.Now, _totalRows);
    Console.WriteLine("[{0}] {1} / {2} (sec.)           ", DateTime.Now, _lastPortionRows, duration.TotalSeconds);
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("Нажмите 'q' для завершения отслеживания изменений...");
}

#endregion
```

С их помощью можно проанализировать какие данные будут выгружены и отказаться от выгрузки с помощью поля "Cancel" в параметре события "BeforeExportDataEventArgs" в событии "Перед экспортом данных". В событии "После экспорта данных" можно проанализировать выгруженные данные.

## Cценарии использования

Библиотека может быть использования для создания приложений для экспорта стандартного журнала регистрации платформы 1С:Предприяние 8.ч в нестандартные хранилища. На текущий момент доступна выгрузка в базы данных PostgreSQL и SQL Server.

Основные цели выгрузки - создать более производительный и эффективный способ работы с журналом регистрации с минимальным риском нарушить штатную работу платформы 1С. Может использоватьяс например для:

* Контроля состояния системы на постоянной основе (периодические рассылки, проверка ошибок в течении рабочего и др.)
* Долгосрочное хранение информации о действиях в информационной базе с удобным способом хранения, бэкапирования и развертывания.
* Создание возможности работать с журналом регистрации с помощью стандартных запросов платформы 1С через внешние источники данных или ADO.
* Возможноть работы с данными журнала регистрации с помощью средств вне экосистемы платформы 1С.

И это не полный список, все зависит от конкретных задач.

## Производительность

Производительность работы библиотеки сбалансирована для работы с достаточно большим объемом данных в журнале регистрации. Также при экспорте данных не нарушается работа платформы 1С, которая продолжает работать как обычно и не подозревает о происходящих процессах выгрузки.

Скорость экспорта записей журнала регистрации зависит от мощности оборудования, загруженности системы и конфигурации инфраструктуры.

При тестировании экспорта данных между двумя серверами (сервер приложений 1С и сервер баз данных с базой для экспорта журнала регистрации) со средней нагрузкой и каналом связи 1 Гбит/сек были получены следующие результаты. Тестовая машина также имела 16 ядер и процессор Intel Core i9900k.

| № | СУБД | Порция данных | Среднее время выгрузки (сек.) | Среднее использование CPU приложением, % | Среднее использование RAM приложением, МБ |
| - | ---- | ------------- | ----------------------------- | ----------------------- | ----------------------- |
| 1 | SQL Server | 10000 | 0.27 | 0.7 | 60 |
| 2 | PostgreSQL | 10000 | 0.32 | 0.8 | 97 |

В целом не важно какая СУБД используется для хранения данных журнала регистрации. Разница в производительности на уровне статистической погрешности. В обоих вариантах время выгрузки около 35 тыс. записей журнала регистрации в минуту. Не часто можно встретить информационную базу, которая генерирует такой объем записей, но и она не будет препятствием для использования этой библиотеки выгрузки.

## TODO

Планы в части разработки:

* Добавить возможность экспорта данных в ElasticSearch
* Добавить возможность экспорта данных в MySQL
* Добавить возможность экспорта данных в MongoDB
* Улучшить обработку ошибок по уровням возникновения (критические и нет)
* Улучшение производительности и добавление bencmark'ов
* Расширение unit-тестов библиотеки

## Лицензия

MIT - делайте все, что посчитаете нужным. Никакой гарантии и никаких ограничений по использованию.

$appsettings = @'
{
  "ConnectionStrings": {
    "EventLogDatabase": "Server=(local)\SQL2017;Database=master;User ID=sa;Password=Password12!"
  },
  "InformationSystem": {
    "Name": "EventLogExportTest",
    "Description": "Тесты проверки экспорта данных журнала регистрации."
  },
  "EventLog": {
    "SourcePath": "TestData\\LGFFormat",
    "UseWatchMode": false,
    "WatchPeriod": 5,
    "Portion": 10000
  }
}
'@;
$appsettings | Out-File "Tests\YY.EventLogExportAssistant.SQLServer.Tests\bin\Release\netcoreapp3.1\appsettings.json"


dotnet test ./Tests/YY.EventLogExportAssistant.Core.Tests/YY.EventLogExportAssistant.Core.Tests.csproj
dotnet test ./Tests/YY.EventLogExportAssistant.SQLServer.Tests/YY.EventLogExportAssistant.SQLServer.Tests.csproj
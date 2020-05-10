#!/bin/sh
API_KEY = $1

dotnet nuget push ./Libs/YY.EventLogExportAssistant.Core/bin/Release/YY.EventLogExportAssistant.Core.*.nupkg -k $1 -s https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push ./Libs/YY.EventLogExportAssistant.SQLServer/bin/Release/YY.EventLogExportAssistant.SQLServer.*.nupkg -k $1 -s https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push ./Libs/YY.EventLogExportAssistant.PostgreSQL/bin/Release/YY.EventLogExportAssistant.PostgreSQL.*.nupkg -k $1 -s https://api.nuget.org/v3/index.json --skip-duplicate
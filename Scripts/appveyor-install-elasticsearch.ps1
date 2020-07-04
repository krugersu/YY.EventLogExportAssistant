Write-Host "Installing JDK 11 ..." -ForegroundColor Cyan

New-Item "${env:ProgramFiles}\Java" -ItemType Directory -Force | Out-Null

$jdkPath = "${env:ProgramFiles}\Java\jdk11"

if(Test-Path $jdkPath) {
    Remove-Item $jdkPath -Recurse -Force
}

Write-Host "Downloading..."
$zipPath = "$env:TEMP\openjdk-11.0.2_windows-x64_bin.zip"
(New-Object Net.WebClient).DownloadFile('https://download.java.net/java/GA/jdk11/9/GPL/openjdk-11.0.2_windows-x64_bin.zip', $zipPath)

Write-Host "Unpacking..."
$tempPath = "$env:TEMP\jdk11_temp"
7z x $zipPath -o"$tempPath" | Out-Null
[IO.Directory]::Move("$tempPath\jdk-11.0.2", $jdkPath)
Remove-Item $tempPath -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item $zipPath -ErrorAction SilentlyContinue

cmd /c "`"$jdkPath\bin\java`" --version"

Write-Host "Java path:"
Write-Host $jdkPath

Write-Host "JAVA_HOME (before):"
Write-Host $env:JAVA_HOME

[Environment]::SetEnvironmentVariable("JAVA_HOME", $jdkPath)

Write-Host "JAVA_HOME (after):"
Write-Host $env:JAVA_HOME

Write-Host "JDK 11 installed" -ForegroundColor Green

Write-Host "Preparing to download and install Elasticsearch..." -ForegroundColor Cyan
$esVersion = "7.8.0"
$downloadUrl = "https://artifacts.elastic.co/downloads/elasticsearch/elasticsearch-$($esVersion)-windows-x86_64.zip"
$zipPath = "$($env:USERPROFILE)\elasticsearch-$esVersion.zip"
$extractRoot = "$env:SYSTEMDRIVE\Elasticsearch"
$esRoot = "$extractRoot\elasticsearch-$esVersion"

Write-Host "Downloading Elasticsearch..."
(New-Object Net.WebClient).DownloadFile($downloadUrl, $zipPath)
7z x $zipPath -y -o"$extractRoot" | Out-Null
del $zipPath

Write-Host "Installing Elasticsearch as a Windows service..."
& "$esRoot\bin\elasticsearch-service.bat" install

Write-Host "Starting Elasticsearch service..."
& "$esRoot\bin\elasticsearch-service.bat" start

do {
  Write-Host "Waiting for Elasticsearch service to bootstrap..."
  sleep 1
} until(Test-NetConnection localhost -Port 9200 | ? { $_.TcpTestSucceeded } )

Write-Host "Elasticsearch installed" -ForegroundColor Green 
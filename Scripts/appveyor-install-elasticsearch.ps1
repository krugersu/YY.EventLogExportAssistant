Write-Host "Preparing to download and install Java..." -ForegroundColor Cyan
$URL=(Invoke-WebRequest -UseBasicParsing https://www.java.com/en/download/manual.jsp).Content | %{[regex]::matches($_, '(?:<a title="Download Java software for Windows Online" href=")(.*)(?:">)').Groups[1].Value}
Invoke-WebRequest -UseBasicParsing -OutFile jre8.exe $URL
Start-Process .\jre8.exe '/s REBOOT=0 SPONSORS=0 AUTO_UPDATE=0' -wait
echo $?
Write-Host "Java installed" -ForegroundColor Green 

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
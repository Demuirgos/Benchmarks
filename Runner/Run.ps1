Write-Output "Runnig Feeder"
Set-Location "../Feeder"
Start-Process dotnet run
Write-Output "Runnig Target"
Set-Location "../Target"
Start-Process dotnet run
Start-Sleep -Seconds 5
Write-Output "Runnig Observer"
Set-Location "../Observer"
Start-Process dotnet run
Split-Path $MyInvocation.MyCommand.Path | Set-Location

Write-Host "Cleaning solution..."
Get-ChildItem -Exclude "*.dll" -Include bin,obj -Recurse | Remove-Item -Force -Recurse
Write-Host "Finished."
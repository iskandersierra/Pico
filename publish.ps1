.\pack.ps1

dotnet nuget push .\packages\*.nupkg --source nuget.org --api-key $env:NUGET_FROM_HOME_TOKEN --skip-duplicate

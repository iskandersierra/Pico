Remove-Item $PSScriptRoot\packages -Recurse -Force -ErrorAction SilentlyContinue

dotnet pack -o $PSScriptRoot\packages

Remove-Item $PSScriptRoot\packages -Recurse -Force -ErrorAction SilentlyContinue

dotnet pack -o $PSScriptRoot\packages -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg

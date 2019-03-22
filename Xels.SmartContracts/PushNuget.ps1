rm "bin\release\" -Recurse -Force
dotnet pack --configuration Release  
dotnet nuget push bin\Release\Xels.SmartContracts.*.nupkg -k  -s https://api.nuget.org/v3/index.json
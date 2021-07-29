rmdir .\ReassureTest.Net\bin\ -Recurse -Force 
dotnet test
dotnet pack --include-source
rmdir .\ReassureTest.Net\bin\ -Recurse -Force 
rmdir .\ReassureTest.Net\obj\ -Recurse -Force 
dotnet test
dotnet pack --include-source
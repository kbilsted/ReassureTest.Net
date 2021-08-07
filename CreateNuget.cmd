rmdir .\ReassureTest.Net\bin /s /q
rmdir .\ReassureTest.Net\obj /s /q
dotnet test
dotnet pack --include-source
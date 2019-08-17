@echo off

cd ./SmeuBase
dotnet ef database update %1 --context SmeuContextMySQL
dotnet ef database update %1 --context SmeuContextSqlite

dotnet ef migrations remove --context SmeuContextMySQL
dotnet ef migrations remove --context SmeuContextSqlite

cd ../
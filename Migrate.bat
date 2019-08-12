@echo off

cd ./SmeuBase
dotnet ef migrations add %1 --context SmeuContextSqlite --output-dir Migrations/Sqlite
dotnet ef database update --context SmeuContextSqlite

dotnet ef migrations add %1 --context SmeuContextMySQL --output-dir Migrations/MySQL
dotnet ef database update --context SmeuContextMySQL
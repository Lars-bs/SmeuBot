@echo off

cd ./SmeuBase
dotnet ef migrations add %1 --context SmeuContextSqlite --output-dir Migrations/Sqlite
dotnet ef migrations add %1 --context SmeuContextMySQL --output-dir Migrations/MySQL
cd ../
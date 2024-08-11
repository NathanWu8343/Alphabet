rmdir /S /Q Migrations

dotnet ef migrations add InitialDb -c ApplicationWriteDbContext -o Migrations

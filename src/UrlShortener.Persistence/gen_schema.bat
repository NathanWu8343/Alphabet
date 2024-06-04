rmdir /S /Q Migrations

dotnet ef migrations add InitialDb -c ApplicationDbContext -o Migrations

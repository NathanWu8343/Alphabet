rmdir /S /Q CompiledModels

dotnet ef dbcontext optimize -c ApplicationDbContext -o CompiledModels -n UrlShortener.Persistence

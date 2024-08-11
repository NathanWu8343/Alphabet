rmdir /S /Q CompiledModels

dotnet ef dbcontext optimize -c ApplicationWriteDbContext -o CompiledModels -n UrlShortener.Persistence

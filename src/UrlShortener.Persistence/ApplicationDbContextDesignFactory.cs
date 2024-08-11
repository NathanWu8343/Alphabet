using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UrlShortener.Persistence
{
    public class ApplicationDbContextDesignFactory : IDesignTimeDbContextFactory<ApplicationWriteDbContext>
    {
        public ApplicationWriteDbContext CreateDbContext(string[] args)
        {
            var serverVersion = new MariaDbServerVersion(new Version(10, 5, 5));
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationWriteDbContext>();
            optionsBuilder.UseMySql("Server=localhost;Port=3306;Database=Alphabet;User Id=root;Password=1234567;Charset=utf8mb4;Convert Zero Datetime=True;", serverVersion);

            return new ApplicationWriteDbContext(optionsBuilder.Options);
        }
    }
}
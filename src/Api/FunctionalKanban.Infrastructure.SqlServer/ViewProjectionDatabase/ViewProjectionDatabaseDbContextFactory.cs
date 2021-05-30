namespace FunctionalKanban.Infrastructure.SqlServer.ViewProjectionDatabase
{
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    public class ViewProjectionDatabaseDbContextFactory : IDesignTimeDbContextFactory<ViewProjectionDatabaseDbContext>
    {
        public ViewProjectionDatabaseDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                 .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "ViewProjectionDatabase"))
                 .AddJsonFile("appsettings.json", true)
                 .AddEnvironmentVariables()
                 .Build();

            var builder = new DbContextOptionsBuilder();

            var connectionString = configuration
                        .GetConnectionString("ViewProjectionDatabaseConnexionString");

            builder.UseSqlServer(connectionString,
                        x => x.MigrationsAssembly(typeof(ViewProjectionDatabaseDbContextFactory).Assembly.FullName));

            return new ViewProjectionDatabaseDbContext(builder.Options);
        }
    }
}

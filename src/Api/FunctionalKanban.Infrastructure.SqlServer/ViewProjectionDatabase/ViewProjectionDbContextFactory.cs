namespace FunctionalKanban.Infrastructure.SqlServer.ViewProjectionDatabase
{
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    public class ViewProjectionDbContextFactory : IDesignTimeDbContextFactory<ViewProjectionDbContext>
    {
        public ViewProjectionDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                 .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "ViewProjectionDatabase"))
                 .AddJsonFile("appsettings.json", true)
                 .AddEnvironmentVariables()
                 .Build();

            var builder = new DbContextOptionsBuilder<ViewProjectionDbContext>();

            var connectionString = configuration
                        .GetConnectionString("ViewProjectionDatabaseConnexionString");

            builder.UseSqlServer(connectionString,
                        x => x.MigrationsAssembly(typeof(ViewProjectionDbContextFactory).Assembly.FullName));

            return new ViewProjectionDbContext(builder.Options);
        }
    }
}

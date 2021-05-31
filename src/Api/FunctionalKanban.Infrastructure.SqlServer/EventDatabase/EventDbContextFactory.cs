namespace FunctionalKanban.Infrastructure.SqlServer.EventDatabase
{
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    public class EventDbContextFactory : IDesignTimeDbContextFactory<EventDbContext>
    {
        public EventDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                 .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "EventDatabase"))
                 .AddJsonFile("appsettings.json", true)
                 .AddEnvironmentVariables()
                 .Build();

            var builder = new DbContextOptionsBuilder<EventDbContext>();

            var connectionString = configuration
                        .GetConnectionString("EventDatabaseConnexionString");

            builder.UseSqlServer(connectionString,
                        x => x.MigrationsAssembly(typeof(EventDbContextFactory).Assembly.FullName));

            return new EventDbContext(builder.Options);
        }
    }
}

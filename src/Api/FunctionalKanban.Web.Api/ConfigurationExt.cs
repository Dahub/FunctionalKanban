namespace FunctionalKanban.Web.Api
{
    using FunctionalKanban.Infrastructure.Abstraction;
    using FunctionalKanban.Infrastructure.Implementation;
    using FunctionalKanban.Infrastructure.SqlServer.EventDatabase;
    using FunctionalKanban.Infrastructure.SqlServer.ViewProjectionDatabase;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ConfigurationExt
    {
        public static IServiceCollection WithSqlServer(this IServiceCollection services, IConfiguration configuration) => services.
            WithEventDbContext(configuration.GetConnectionString("EventDatabaseConnexionString")).
            WithViewProjectionDbContextDbContext(configuration.GetConnectionString("ViewProjectionDatabaseConnexionString")).
            WithEventDatabase().
            WithViewProjectionDatabase();

        public static IServiceCollection WithRepositories(this IServiceCollection services) => services.
            AddScoped<IEntityStateRepository, EntityStateRepository>().
            AddScoped<IViewProjectionRepository, ViewProjectionRepository>();

        private static IServiceCollection WithEventDbContext(this IServiceCollection services, string connexionString) =>
            services.AddDbContext<EventDbContext>(options => options.UseSqlServer(connexionString));

        private static IServiceCollection WithViewProjectionDbContextDbContext(this IServiceCollection services, string connexionString) =>
            services.AddDbContext<ViewProjectionDbContext>(options => options.UseSqlServer(connexionString));

        private static IServiceCollection WithEventDatabase(this IServiceCollection services) =>
            services.AddScoped<IEventDataBase, SqlServerEventDatabase>();

        private static IServiceCollection WithViewProjectionDatabase(this IServiceCollection services) =>
            services.AddScoped<IViewProjectionDataBase, SqlServerViewProjectionDatabase>();
    }
}

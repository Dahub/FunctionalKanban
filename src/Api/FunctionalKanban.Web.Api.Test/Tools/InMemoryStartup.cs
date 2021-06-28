namespace FunctionalKanban.Web.Api.Test.Tools
{
    using FunctionalKanban.Infrastructure.Abstraction;
    using FunctionalKanban.Infrastructure.InMemory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    internal class InMemoryStartup : Startup, ITestStartup
    {
        public InMemoryDatabase ViewProjectionDataBase { get; set; }

        public InMemoryDatabase EventDataBase { get; set; }

        public InMemoryStartup(IConfiguration configuration) : base(configuration) { }

        protected override IServiceCollection ConfigureDatabases(IServiceCollection services) => services.
            AddScoped<IEventDataBase>(_ => EventDataBase).
            AddScoped<IViewProjectionDataBase>(_ => ViewProjectionDataBase);
    }
}

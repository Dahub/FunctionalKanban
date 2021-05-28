namespace FunctionalKanban.Web.Api.Test.Tools
{
    using FunctionalKanban.Infrastructure.Abstraction;
    using FunctionalKanban.Infrastructure.InMemory;
    using Microsoft.Extensions.Configuration;

    internal class InMemoryStartup : Startup, ITestStartup
    {
        public InMemoryDatabase ViewProjectionDataBase { get; set; }

        public InMemoryDatabase EventDataBase { get; set; }

        public InMemoryStartup(IConfiguration configuration) : base(configuration) { }

        protected override IDatabaseFactory GetDatabaseFactory() => 
            new InMemoryDatabaseFactory(EventDataBase, ViewProjectionDataBase);
    }
}

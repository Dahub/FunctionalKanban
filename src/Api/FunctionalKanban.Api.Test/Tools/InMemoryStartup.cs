namespace FunctionalKanban.Api.Test.Tools
{
    using FunctionalKanban.Infrastructure.Abstraction;
    using FunctionalKanban.Infrastructure.InMemory;
    using Microsoft.Extensions.Configuration;

    internal class InMemoryStartup : Startup, ITestStartup
    {
        public InMemoryDatabase ViewProjectionDataBase { get; set; }

        public InMemoryDatabase EventDataBase { get; set; }

        public InMemoryStartup(IConfiguration configuration) : base(configuration) { }

        protected override IDatabaseFactory BuildDatabaseFactory() => 
            new InMemoryDatabaseFactory(EventDataBase, ViewProjectionDataBase);
    }
}

namespace FunctionalKanban.Infrastructure.SqlServer
{
    using FunctionalKanban.Infrastructure.Abstraction;
    using FunctionalKanban.Infrastructure.InMemory;

    public class SqlServerEfContextFactory : IDatabaseFactory
    {
        public IEventDataBase GetEventDatabase() => new InMemoryDatabase();

        public IViewProjectionDataBase GetViewProjectionDatabase() => new InMemoryDatabase();
    }
}

namespace FunctionalKanban.Infrastructure.InMemory
{
    using FunctionalKanban.Infrastructure.Abstraction;

    public class InMemoryDatabaseFactory : IDatabaseFactory
    {
        private readonly IEventDataBase _eventDataBase;

        private readonly IViewProjectionDataBase _viewProjectionDataBase;

        public InMemoryDatabaseFactory()
        {
            _eventDataBase = new InMemoryDatabase();
            _viewProjectionDataBase = new InMemoryDatabase();
        }

        public InMemoryDatabaseFactory(
            IEventDataBase eventDatabase,
            IViewProjectionDataBase viewProjectionDatabase)
        {
            _eventDataBase = eventDatabase;
            _viewProjectionDataBase = viewProjectionDatabase;
        }

        public IEventDataBase GetEventDatabase() => _eventDataBase;

        public IViewProjectionDataBase GetViewProjectionDatabase() => _viewProjectionDataBase;
    }
}

namespace FunctionalKanban.Infrastructure.SqlServer
{
    using System;
    using FunctionalKanban.Infrastructure.Abstraction;

    public class SqlServerEfContextFactory : IDatabaseFactory
    {
        private readonly string _eventDatabaseConnexionString;

        private readonly string _viewProjectionDatabaseConnexionString;

        public SqlServerEfContextFactory(
            string eventDatabaseConnexionString,
            string viewProjectionDatabaseConnexionString)
        {
            _eventDatabaseConnexionString = eventDatabaseConnexionString;
            _viewProjectionDatabaseConnexionString = viewProjectionDatabaseConnexionString;
        }

        public IEventDataBase CreateEventDatabase() => throw new NotImplementedException();

        public IViewProjectionDataBase CreateViewProjectionDatabase() => throw new NotImplementedException();
    }
}

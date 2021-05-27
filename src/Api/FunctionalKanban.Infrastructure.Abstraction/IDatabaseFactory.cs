namespace FunctionalKanban.Infrastructure.Abstraction
{
    public interface IDatabaseFactory
    {
        IEventDataBase CreateEventDatabase();

        IViewProjectionDataBase CreateViewProjectionDatabase();
    }
}

namespace FunctionalKanban.Infrastructure.Abstraction
{
    public interface IDatabaseFactory
    {
        IEventDataBase GetEventDatabase();

        IViewProjectionDataBase GetViewProjectionDatabase();
    }
}

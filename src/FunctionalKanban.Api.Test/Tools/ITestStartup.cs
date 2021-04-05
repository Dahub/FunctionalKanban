namespace FunctionalKanban.Api.Test.Tools
{
    using FunctionalKanban.Infrastructure.InMemory;

    public interface ITestStartup
    {
        InMemoryDatabase DataBase { get; set; }
    }
}

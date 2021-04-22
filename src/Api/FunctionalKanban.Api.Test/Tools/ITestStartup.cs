namespace FunctionalKanban.Api.Test.Tools
{
    using FunctionalKanban.Infrastructure.InMemory;

    public interface ITestStartup
    {
        InMemoryDatabase ViewProjectionDataBase { get; set; }

        InMemoryDatabase EventDataBase { get; set; }
    }
}

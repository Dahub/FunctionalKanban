namespace FunctionalKanban.Api.Test.Tools
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Infrastructure.InMemory;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;

    public abstract class BaseTestClass
    {
        protected static HttpClient BuildNewHttpClient<T>(
            InMemoryDatabase eventDataBase,
            InMemoryDatabase viewProjectionDataBase) where T : class, ITestStartup
        {
            var builder = new WebHostBuilder();

            var config = new ConfigurationBuilder()
                    .Build();

            var server = new TestServer(builder
                .UseConfiguration(config)
                .UseStartup((w) => BuildTestStartup<T>(eventDataBase, viewProjectionDataBase, w))
                .UseEnvironment("test"));

            var httpWebClient = server.CreateClient();

            return httpWebClient;
        }

        private static ITestStartup BuildTestStartup<T>(
            InMemoryDatabase eventDataBase,
            InMemoryDatabase viewProjectionDataBase,
            WebHostBuilderContext context)
        {
            var startup = (ITestStartup)Activator.CreateInstance(typeof(T), new object[] { context.Configuration });
            startup.EventDataBase = eventDataBase;
            startup.ViewProjectionDataBase = viewProjectionDataBase;
            return startup;
        }
        protected static async Task InitNewTask(HttpClient httpClient, Guid entityId) =>
            _ = await httpClient
                .PostAsJsonAsync(
                    "task",
                    new CreateTask()
                    {
                        EntityId = entityId,
                        Name = Guid.NewGuid().ToString(),
                        RemaningWork = 10
                    });
    }
}

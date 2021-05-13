namespace FunctionalKanban.Api.Test.Tools
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using FunctionalKanban.Domain.Project.Commands;
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

        protected static async Task InitLinkTaskToProject(
                HttpClient httpClient,
                Guid taskId,
                Guid projectId) =>
            _ = await httpClient.PostAsJsonAsync(
                    "/task/linkToProject",
                    new LinkToProject()
                    {
                        EntityId = taskId,
                        ProjectId = projectId
                    });

        protected static async Task InitNewTask(
                HttpClient httpClient, 
                Guid entityId, 
                uint remaningWork = 10) =>
            _ = await httpClient
                .PostAsJsonAsync(
                    "task",
                    new CreateTask()
                    {
                        EntityId = entityId,
                        Name = Guid.NewGuid().ToString(),
                        RemaningWork = remaningWork
                    });

        protected static async Task InitNewProject(HttpClient httpClient, Guid entityId) =>
           _ = await httpClient
               .PostAsJsonAsync(
                   "project",
                   new CreateProject()
                   {
                       EntityId = entityId,
                       Name = Guid.NewGuid().ToString()
                   });
    }
}

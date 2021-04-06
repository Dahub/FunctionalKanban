namespace FunctionalKanban.Api.Test
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FunctionalKanban.Api.Test.Tools;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Infrastructure.InMemory;
    using Xunit;

    public class PostChangeTaskStatusShould : BaseTestClass
    {
        [Fact]
        public async Task ReturnHttpOkWhenPostChangeTaskStatus()
        {
            var entityId = Guid.NewGuid();
            var dataBase = new InMemoryDatabase();

            var httpClient = BuildNewHttpClient<InMemoryStartup>(dataBase);

            await InitNewTask(httpClient, entityId);
            
            var httpResponseMessage = await httpClient
                .PostAsJsonAsync(
                    "task/changeStatus",
                    new ChangeTaskStatus()
                    {
                        AggregateId = entityId,
                        TaskStatus = Domain.Task.TaskStatus.InProgress
                    }); 

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            var lines = dataBase.EventLines.Where(e => e.AggregateId.Equals(entityId));

            lines.Should().HaveCount(2);
            lines.FirstOrDefault(e => e.Version.Equals(2)).Should().NotBeNull();
        }

        private static async Task InitNewTask(HttpClient httpClient, Guid entityId) => 
            _ = await httpClient
                .PostAsJsonAsync(
                    "task",
                    new CreateTask()
                    {
                        AggregateId = entityId,
                        Name = Guid.NewGuid().ToString(),
                        RemaningWork = 10
                    });
    }
}

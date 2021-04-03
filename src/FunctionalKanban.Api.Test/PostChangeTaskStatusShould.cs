namespace FunctionalKanban.Api.Test
{
    using System;
    using System.Linq;
    using System.Net;
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
            InMemoryDatabase.Reset();
            var entityId = Guid.NewGuid();

            await InitNewTask(entityId);

            var httpClient = BuildNewHttpClient<InMemoryStartup>();
            var httpResponseMessage = await httpClient
                .PostAsJsonAsync(
                    "task/changeStatus",
                    new ChangeTaskStatus()
                    {
                        AggregateId = entityId,
                        TaskStatus = Domain.Task.TaskStatus.InProgress
                    });

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            InMemoryDatabase.EventLines.Should().HaveCount(2);
            InMemoryDatabase.EventLines.FirstOrDefault(e => e.version.Equals(2)).Should().NotBeNull();
        }

        private async Task InitNewTask(Guid entityId)
        {
            var httpClient = BuildNewHttpClient<InMemoryStartup>();
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
}

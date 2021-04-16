namespace FunctionalKanban.Api.Test
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Json;
    using FluentAssertions;
    using FunctionalKanban.Api.Test.Tools;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Infrastructure.InMemory;
    using Xunit;

    public class PostDeleteTaskShould : BaseTestClass
    {
        [Fact]
        public async void DeleteTask()
        {
            var entityId = Guid.NewGuid();
            var eventDataBase = new InMemoryDatabase();

            var httpClient = BuildNewHttpClient<InMemoryStartup>(eventDataBase, new InMemoryDatabase());

            await InitNewTask(httpClient, entityId);

            var httpResponseMessage = await httpClient
                .PostAsJsonAsync(
                    "task/delete",
                    new DeleteTask()
                    {
                        EntityId = entityId
                    });

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            var lines = eventDataBase.Events.Where(e => e.EntityId.Equals(entityId));
            lines.Should().HaveCount(2);
            lines.FirstOrDefault(e => e.EntityVersion.Equals(2)).Should().NotBeNull();
        }

        [Fact]
        public async void DeleteTaskInTaskViewProjection()
        {
            var entityId = Guid.NewGuid();
            var dataBase = new InMemoryDatabase();

            var httpClient = BuildNewHttpClient<InMemoryStartup>( new InMemoryDatabase(), dataBase);

            await InitNewTask(httpClient, entityId);

            dataBase.TaskViewProjections.Any(t => t.Id.Equals(entityId)).Should().BeTrue();

            _ = await httpClient
                .PostAsJsonAsync(
                    "task/delete",
                    new DeleteTask()
                    {
                        EntityId = entityId
                    });

            dataBase.TaskViewProjections.Any(t => t.Id.Equals(entityId)).Should().BeFalse();
        }
    }
}

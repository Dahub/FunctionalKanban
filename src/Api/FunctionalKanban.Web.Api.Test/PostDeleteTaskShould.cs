namespace FunctionalKanban.Web.Api.Test
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Json;
    using FluentAssertions;
    using FunctionalKanban.Web.Api.Test.Tools;
    using FunctionalKanban.Core.Domain.Task.Commands;
    using FunctionalKanban.Core.Domain.ViewProjections;
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

            var lines = eventDataBase.EventsByEntityId(entityId);
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

            dataBase.GetProjections<TaskViewProjection>().Any(t => t.Id.Equals(entityId)).Should().BeTrue();

            _ = await httpClient
                .PostAsJsonAsync(
                    "task/delete",
                    new DeleteTask()
                    {
                        EntityId = entityId
                    });

            dataBase.GetProjections<TaskViewProjection>().Any(t => t.Id.Equals(entityId)).Should().BeFalse();
        }

        [Fact]
        public async void PopulateDeletedTaskViewProjection()
        {
            var entityId = Guid.NewGuid();
            var dataBase = new InMemoryDatabase();

            var httpClient = BuildNewHttpClient<InMemoryStartup>(new InMemoryDatabase(), dataBase);

            await InitNewTask(httpClient, entityId);

            _ = await httpClient
                .PostAsJsonAsync(
                    "task/delete",
                    new DeleteTask()
                    {
                        EntityId = entityId
                    });

            dataBase.GetProjections<DeletedTaskViewProjection>().Where(t => t.Id.Equals(entityId)).Should().HaveCount(1);
        }
    }
}

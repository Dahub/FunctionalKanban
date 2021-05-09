namespace FunctionalKanban.Api.Test
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Json;
    using FluentAssertions;
    using FunctionalKanban.Api.Test.Tools;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Domain.Task.Events;
    using FunctionalKanban.Domain.ViewProjections;
    using FunctionalKanban.Infrastructure.InMemory;
    using Xunit;

    public class PostChangeTaskRemaningWorkShould : BaseTestClass
    {
        [Fact]
        public async void ChangeRemaningWork()
        {
            var entityId = Guid.NewGuid();
            var expectedRemaningWork = 5u;
            var eventDataBase = new InMemoryDatabase();

            var httpClient = BuildNewHttpClient<InMemoryStartup>(eventDataBase, new InMemoryDatabase());

            await InitNewTask(httpClient, entityId, 10u);

            var httpResponseMessage = await httpClient
                .PostAsJsonAsync(
                    "task/changeRemaningWork",
                    new ChangeRemaningWork()
                    {
                        EntityId = entityId,
                        RemaningWork = expectedRemaningWork
                    });

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            var lines = eventDataBase.Events.Where(e => e.EntityId.Equals(entityId));
            lines.Should().HaveCount(2);

            var lastEvent = lines.FirstOrDefault(e => e.EntityVersion.Equals(2));

            lastEvent.Should().NotBeNull();
            lastEvent.Should().BeOfType<TaskRemaningWorkChanged>();

            ((TaskRemaningWorkChanged)lastEvent).RemaningWork.Should().Be(expectedRemaningWork);
        }

        [Fact]
        public async void UpdateRemaningWorkInViewProjection()
        {
            var entityId = Guid.NewGuid();
            var expectedRemaningWork = 5u;
            var dataBase = new InMemoryDatabase();

            var httpClient = BuildNewHttpClient<InMemoryStartup>(new InMemoryDatabase(), dataBase);

            await InitNewTask(httpClient, entityId, 10u);

            _ = await httpClient
                .PostAsJsonAsync(
                    "task/changeRemaningWork",
                    new ChangeRemaningWork()
                    {
                        EntityId = entityId,
                        RemaningWork = expectedRemaningWork
                    });

            dataBase.GetProjections<TaskViewProjection>().
                Single(v => v.Id.Equals(entityId)).RemaningWork.Should().Be(expectedRemaningWork);
        }
    }
}

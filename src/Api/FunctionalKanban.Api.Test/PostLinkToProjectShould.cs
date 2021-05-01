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
    using FunctionalKanban.Infrastructure.InMemory;
    using Xunit;
    using static LaYumba.Functional.F;

    public class PostLinkToProjectShould : BaseTestClass
    {
        [Fact]
        public async void AddTaskToProjectRelatedTasks()
        {

        }

        [Fact]
        public async void LinkTaskToProject()
        {
            var entityId = Guid.NewGuid();
            var expectedProjectId = Guid.NewGuid();
            var eventDataBase = new InMemoryDatabase();

            var httpClient = BuildNewHttpClient<InMemoryStartup>(eventDataBase, new InMemoryDatabase());

            await InitNewTask(httpClient, entityId);

            var httpResponseMessage = await httpClient
                .PostAsJsonAsync(
                    "task/linkToProject",
                    new LinkToProject()
                    {
                        EntityId = entityId,
                        ProjectId = expectedProjectId
                    });

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            var lines = eventDataBase.Events.Where(e => e.EntityId.Equals(entityId));
            lines.Should().HaveCount(2);

            var lastEvent = lines.FirstOrDefault(e => e.EntityVersion.Equals(2));

            lastEvent.Should().NotBeNull();
            lastEvent.Should().BeOfType<TaskLinkedToProject>();

            ((TaskLinkedToProject)lastEvent).ProjectId.Should().Be(Some(expectedProjectId));
        }

        [Fact]
        public async void UpdateProjectIdInViewProjection()
        {
            var entityId = Guid.NewGuid();
            var expectedProjectId = Guid.NewGuid();
            var dataBase = new InMemoryDatabase();

            var httpClient = BuildNewHttpClient<InMemoryStartup>(new InMemoryDatabase(), dataBase);

            await InitNewTask(httpClient, entityId);

            _ = await httpClient
                .PostAsJsonAsync(
                    "task/linkToProject",
                    new LinkToProject()
                    {
                        EntityId = entityId,
                        ProjectId = expectedProjectId
                    });

            dataBase.TaskViewProjections.Single(v => v.Id.Equals(entityId)).ProjectId.Should().Be(Some(expectedProjectId));
        }
    }
}

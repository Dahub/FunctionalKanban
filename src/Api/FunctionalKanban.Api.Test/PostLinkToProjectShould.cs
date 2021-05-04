namespace FunctionalKanban.Api.Test
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Json;
    using FluentAssertions;
    using FunctionalKanban.Api.Test.Tools;
    using FunctionalKanban.Domain.Project.Events;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Domain.Task.Events;
    using FunctionalKanban.Infrastructure.InMemory;
    using Xunit;
    using static LaYumba.Functional.F;

    public class PostLinkToProjectShould : BaseTestClass
    {
        [Fact]
        public async void LinkTaskToProject()
        {
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var eventDataBase = new InMemoryDatabase();

            var httpClient = BuildNewHttpClient<InMemoryStartup>(eventDataBase, new InMemoryDatabase());

            await InitNewTask(httpClient, taskId);
            await InitNewProject(httpClient, projectId);

            var httpResponseMessage = await httpClient
                .PostAsJsonAsync(
                    "task/linkToProject",
                    new LinkToProject()
                    {
                        EntityId = taskId,
                        ProjectId = projectId
                    });

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            var lines = eventDataBase.Events.Where(e => e.EntityId.Equals(taskId));
            lines.Should().HaveCount(2);

            var taskLinkToProjectEvent = lines.FirstOrDefault(e => e is TaskLinkedToProject);
            taskLinkToProjectEvent.Should().NotBeNull();
            ((TaskLinkedToProject)taskLinkToProjectEvent).ProjectId.Should().Be(Some(projectId));

            lines = eventDataBase.Events.Where(e => e.EntityId.Equals(projectId));

            var projectNewTaskLinkedEvent = lines.FirstOrDefault(e => e is ProjectNewTaskLinked);
            projectNewTaskLinkedEvent.Should().NotBeNull();
            ((ProjectNewTaskLinked)projectNewTaskLinkedEvent).TaskId.Should().Be(taskId);
        }

        [Fact]
        public async void UpdateProjectIdInViewProjection()
        {
            var entityId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var dataBase = new InMemoryDatabase();

            var httpClient = BuildNewHttpClient<InMemoryStartup>(new InMemoryDatabase(), dataBase);

            await InitNewTask(httpClient, entityId);
            await InitNewProject(httpClient, projectId);

            _ = await httpClient
                .PostAsJsonAsync(
                    "task/linkToProject",
                    new LinkToProject()
                    {
                        EntityId = entityId,
                        ProjectId = projectId
                    });

            dataBase.TaskViewProjections.Single(v => v.Id.Equals(entityId)).ProjectId.Should().Be(Some(projectId));
        }
    }
}

namespace FunctionalKanban.Web.Api.Test
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Json;
    using FluentAssertions;
    using FunctionalKanban.Web.Api.Test.Tools;
    using FunctionalKanban.Domain.Project.Commands;
    using FunctionalKanban.Domain.ViewProjections;
    using FunctionalKanban.Infrastructure.InMemory;
    using Xunit;

    public class PostDeleteProjectShould : BaseTestClass
    {
        [Fact]
        public async void DeleteProjectButNotChildrenTaskWhenDeleteWithDeleteChildrenFalse()
        {
            var eventDataBase = new InMemoryDatabase();
            var projectionDateBase = new InMemoryDatabase();

            var httpClient = BuildNewHttpClient<InMemoryStartup>(eventDataBase, projectionDateBase);

            var projectId = Guid.NewGuid();
            var firstTaskId = Guid.NewGuid();
            var secondTaskId = Guid.NewGuid();

            await InitNewProject(httpClient, projectId);
            await InitNewTask(httpClient, firstTaskId);
            await InitNewTask(httpClient, secondTaskId);
            await InitLinkTaskToProject(httpClient, firstTaskId, projectId);
            await InitLinkTaskToProject(httpClient, secondTaskId, projectId);

            projectionDateBase.GetProjections<TaskViewProjection>().Any(t => t.Id.Equals(firstTaskId)).Should().BeTrue();
            projectionDateBase.GetProjections<TaskViewProjection>().Any(t => t.Id.Equals(secondTaskId)).Should().BeTrue();
            projectionDateBase.GetProjections<ProjectViewProjection>().Any(t => t.Id.Equals(projectId)).Should().BeTrue();

            var httpResponseMessage = await httpClient
               .PostAsJsonAsync(
                   "project/delete",
                   new DeleteProject()
                   {
                       EntityId = projectId,
                       DeleteChildrenTasks = false
                   });

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            projectionDateBase.GetProjections<TaskViewProjection>().Any(t => t.Id.Equals(firstTaskId)).Should().BeTrue();
            projectionDateBase.GetProjections<TaskViewProjection>().Any(t => t.Id.Equals(secondTaskId)).Should().BeTrue();
            projectionDateBase.GetProjections<ProjectViewProjection>().Any(t => t.Id.Equals(projectId)).Should().BeFalse();
        }

        [Fact]
        public async void DeleteProjectAndChildrenTaskWhenDeleteWithDeleteChildrenTrue()
        {
            var eventDataBase = new InMemoryDatabase();
            var projectionDateBase = new InMemoryDatabase();

            var httpClient = BuildNewHttpClient<InMemoryStartup>(eventDataBase, projectionDateBase);

            var projectId = Guid.NewGuid();
            var firstTaskId = Guid.NewGuid();
            var secondTaskId = Guid.NewGuid();

            await InitNewProject(httpClient, projectId);
            await InitNewTask(httpClient, firstTaskId);
            await InitNewTask(httpClient, secondTaskId);
            await InitLinkTaskToProject(httpClient, firstTaskId, projectId);
            await InitLinkTaskToProject(httpClient, secondTaskId, projectId);

            projectionDateBase.GetProjections<TaskViewProjection>().Any(t => t.Id.Equals(firstTaskId)).Should().BeTrue();
            projectionDateBase.GetProjections<TaskViewProjection>().Any(t => t.Id.Equals(secondTaskId)).Should().BeTrue();
            projectionDateBase.GetProjections<ProjectViewProjection>().Any(t => t.Id.Equals(projectId)).Should().BeTrue();

            var httpResponseMessage = await httpClient
               .PostAsJsonAsync(
                   "project/delete",
                   new DeleteProject()
                   {
                       EntityId = projectId,
                       DeleteChildrenTasks = true
                   });

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            projectionDateBase.GetProjections<TaskViewProjection>().Any(t => t.Id.Equals(firstTaskId)).Should().BeFalse();
            projectionDateBase.GetProjections<TaskViewProjection>().Any(t => t.Id.Equals(secondTaskId)).Should().BeFalse();
            projectionDateBase.GetProjections<ProjectViewProjection>().Any(t => t.Id.Equals(projectId)).Should().BeFalse();
        }
    }
}

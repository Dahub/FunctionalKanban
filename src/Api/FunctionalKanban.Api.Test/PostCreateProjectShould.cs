namespace FunctionalKanban.Api.Test
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FunctionalKanban.Api.Test.Tools;
    using FunctionalKanban.Domain.Project;
    using FunctionalKanban.Domain.Project.Commands;
    using FunctionalKanban.Infrastructure.InMemory;
    using Xunit;

    public class PostCreateProjectShould : BaseTestClass
    {
        [Fact]
        public async Task ReturnHttpOkWhenPostCreateTaskCommand()
        {
            var httpClient = BuildNewHttpClient<DoNothingStartup>(
                new InMemoryDatabase(), new InMemoryDatabase());

            var httpResponseMessage = await httpClient
                .PostAsJsonAsync(
                    "project",
                    new CreateProject()
                    {
                        EntityId = Guid.NewGuid(),
                        Name = Guid.NewGuid().ToString()
                    });

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task PopulateTaskViewProjection()
        {
            var viewProjectionDataBase = new InMemoryDatabase();
            var httpClient = BuildNewHttpClient<InMemoryStartup>(
                new InMemoryDatabase(), viewProjectionDataBase);

            var expectedEntityId = Guid.NewGuid();
            var expectedName = Guid.NewGuid().ToString();
            var expectedStatus = ProjectStatus.New;
            var expectedTotalRemaningWork = 0u;

            _ = await httpClient
                .PostAsJsonAsync(
                    "project",
                    new CreateProject()
                    {
                        EntityId = expectedEntityId,
                        Name = expectedName
                    });

            var projectViewProjections = viewProjectionDataBase.ProjectViewProjections.Where(p => p.Id.Equals(expectedEntityId));

            projectViewProjections.Should().HaveCount(1);

            var projectViewProjection = projectViewProjections.Single();

            projectViewProjection.Name.Should().Be(expectedName);
            projectViewProjection.Status.Should().Be(expectedStatus);
            projectViewProjection.TotalRemaningWork.Should().Be(expectedTotalRemaningWork);
        }
    }
}

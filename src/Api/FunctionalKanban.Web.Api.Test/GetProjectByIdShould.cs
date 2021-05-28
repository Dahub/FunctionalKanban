namespace FunctionalKanban.Web.Api.Test
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FunctionalKanban.Web.Api.Test.Tools;
    using FunctionalKanban.Core.Domain.Project.Commands;
    using FunctionalKanban.Infrastructure.InMemory;
    using Xunit;

    public class GetProjectByIdShould : BaseTestClass
    {
        [Fact]
        public async Task GetProjectWhenProjectExist()
        {
            var expectedResult = "[{\"name\":\"test project\",\"status\":0,\"isDeleted\":false,\"id\":\"6eb4c342-8ed3-4ad3-9d91-119539ce6a6b\"}]";
            var projectId = Guid.Parse("6eb4c342-8ed3-4ad3-9d91-119539ce6a6b");

            var httpClient = BuildNewHttpClient<InMemoryStartup>(
                new InMemoryDatabase(), new InMemoryDatabase());

            await InitNewProject(httpClient, projectId);

            var httpResponseMessage = await httpClient
                .GetAsync($"project/{projectId}");

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

            responseContent.Should().Be(expectedResult);
        }

        private static async Task InitNewProject(
           HttpClient httpClient,
           Guid entityId,
           string entityName = "test project") =>
           _ = await httpClient
               .PostAsJsonAsync(
                   "project",
                   new CreateProject()
                   {
                       EntityId = entityId,
                       Name = entityName
                   });
    }
}

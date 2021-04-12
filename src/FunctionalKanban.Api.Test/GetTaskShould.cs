namespace FunctionalKanban.Api.Test
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FunctionalKanban.Api.Test.Tools;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Infrastructure.InMemory;
    using Xunit;

    public class GetTaskShould : BaseTestClass
    {
        [Fact]
        public async Task ReturnAllTaskWhenGetWithoutParameters()
        {
            var expectedResult = "[{\"name\":\"test task\",\"remaningWork\":0,\"status\":0,\"isDeleted\":false,\"projectId\":null,\"id\":\"6eb4c342-8ed3-4ad3-9d91-119539ce6a6b\"},{\"name\":\"test task\",\"remaningWork\":0,\"status\":0,\"isDeleted\":false,\"projectId\":null,\"id\":\"716b6d2a-a19b-4840-92de-6699ac96d65c\"}]";

            var httpClient = BuildNewHttpClient<InMemoryStartup>(
                new InMemoryDatabase(), new InMemoryDatabase());

            await InitNewTask(httpClient, Guid.Parse("6eb4c342-8ed3-4ad3-9d91-119539ce6a6b"));
            await InitNewTask(httpClient, Guid.Parse("716b6d2a-a19b-4840-92de-6699ac96d65c"));

            var httpResponseMessage = await httpClient
                .GetAsync("task");

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

            responseContent.Should().Be(expectedResult);
        }

        [Fact]
        public async Task ReturnFilteredTaskWhenGetWithParameter()
        {
            var expectedResult = "[{\"name\":\"test 2\",\"remaningWork\":10,\"status\":0,\"isDeleted\":false,\"projectId\":null,\"id\":\"716b6d2a-a19b-4840-92de-6699ac96d65c\"}]";

            var httpClient = BuildNewHttpClient<InMemoryStartup>(
                new InMemoryDatabase(), new InMemoryDatabase());

            await InitNewTask(httpClient, Guid.Parse("6eb4c342-8ed3-4ad3-9d91-119539ce6a6b"), "test 1", 5);
            await InitNewTask(httpClient, Guid.Parse("716b6d2a-a19b-4840-92de-6699ac96d65c"), "test 2", 10);
            await InitNewTask(httpClient, Guid.Parse("8cf17e84-8a24-4a5c-a07b-48628120d193"), "test 3", 20);

            var httpResponseMessage = await httpClient
                .GetAsync("task?minRemaningWork=7&maxRemaningWork=13");

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

            responseContent.Should().Be(expectedResult);
        }

        private static async Task InitNewTask(
            HttpClient httpClient, 
            Guid entityId,
            string entityName = "test task",
            uint remaningWork = 0) =>
            _ = await httpClient
                .PostAsJsonAsync(
                    "task",
                    new CreateTask()
                    {
                        AggregateId = entityId,
                        Name = entityName,
                        RemaningWork = remaningWork,
                        ProjectId = null
                    });
    }
}

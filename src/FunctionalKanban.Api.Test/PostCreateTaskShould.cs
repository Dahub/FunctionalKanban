namespace FunctionalKanban.Api.Test
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using FunctionalKanban.Domain.Task.Commands;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Xunit;
    using FluentAssertions;
    using FunctionalKanban.Infrastructure;
    using FunctionalKanban.Domain.Task;
    using System.Linq;

    public class PostCreateTaskShould
    {
        [Fact]
        public async Task ReturnHttpCreatedWhenPostCreateTaskCommand()
        {
            var httpClient = BuildNewHttpClient<DoNothingStartup>();
            var httpResponseMessage = await httpClient
                .PostAsJsonAsync(
                    "task",
                    new CreateTask()
                    {
                        AggregateId = Guid.NewGuid(),
                        Name = Guid.NewGuid().ToString(),
                        RemaningWork = 10,
                        TimeStamp = DateTime.Now
                    });

            httpResponseMessage.StatusCode.Should().Equals(HttpStatusCode.Created);
        }

        [Fact]
        public async Task ReturnHttpBadRequestWhenPostCreateTaskCommandWithNull()
        {
            var httpClient = BuildNewHttpClient<DoNothingStartup>();
            var httpResponseMessage = await httpClient
                .PostAsJsonAsync<object>(
                    "task", null);

            httpResponseMessage.StatusCode.Should().Equals(HttpStatusCode.BadRequest);

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            responseContent.Should().Be("[\"Commande non prise en charge\"]");
        }

        [Fact]
        public async Task ReturnHttpBadRequestWhenPostCreateTaskCommandWithWrongCommand()
        {
            var httpClient = BuildNewHttpClient<DoNothingStartup>();
            var httpResponseMessage = await httpClient
                .PostAsJsonAsync(
                    "task", Guid.NewGuid());
         
            httpResponseMessage.StatusCode.Should().Equals(HttpStatusCode.BadRequest);

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            responseContent.Should().Be("[\"Les données de la requête ne sont pas serialisables en commande CreateTask\"]");
        }

        [Fact]
        public async Task ReturnHttpBadRequestWhenPostCreateTaskCommandWithIncorrectCommand()
        {
            var httpClient = BuildNewHttpClient<DoNothingStartup>();
            var httpResponseMessage = await httpClient
                .PostAsJsonAsync("task", new CreateTask());

            httpResponseMessage.StatusCode.Should().Equals(HttpStatusCode.BadRequest);

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            responseContent.Should().Be("[\"L'id d'entity doit être défini\",\"La tâche dans avoir un nom\",\"Le time stamp doit être défini\"]");
        }

        [Fact]
        public async Task ReturnHttpInternalServerErrorWhenPostSameTask()
        {
            var eventStream = new InMemoryEventStream();
            InMemoryStartup.EventStream = eventStream;
            var httpClient = BuildNewHttpClient<InMemoryStartup>();

            var createTaskCommand = new CreateTask()
            {
                AggregateId = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                RemaningWork = 10,
                TimeStamp = DateTime.Now
            };

            _ = await httpClient
                .PostAsJsonAsync(
                    "task",createTaskCommand);

            var httpResponseMessage = await httpClient
                .PostAsJsonAsync(
                    "task", createTaskCommand);

            httpResponseMessage.StatusCode.Should().Equals(HttpStatusCode.InternalServerError);

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            responseContent.Should().Be("\"Un événement pour cette version d'aggregat est déjà présent\"");
        }

        [Fact]
        public async Task AddTaskeCreatedEvent()
        {
            var eventStream = new InMemoryEventStream();
            InMemoryStartup.EventStream = eventStream;
            var httpClient = BuildNewHttpClient<InMemoryStartup>();

            var expectedAggregateId = Guid.NewGuid();
            var expectedTimeStamp = DateTime.Now;
            var expectedAggregateName = typeof(TaskEntity).Name;
            var expectedVersion = 1;

            _ = await httpClient
                .PostAsJsonAsync(
                    "task",
                    new CreateTask()
                    {
                        AggregateId = expectedAggregateId,
                        Name = Guid.NewGuid().ToString(),
                        RemaningWork = 10,
                        TimeStamp = expectedTimeStamp
                    });

            eventStream.EventLines.Should().HaveCount(1);

            var eventLine = eventStream.EventLines.Single();

            eventLine.version.Should().Equals(expectedVersion);
            eventLine.aggregateId.Should().Equals(expectedAggregateId);
            eventLine.aggregateName.Should().Equals(expectedAggregateName);
        }

        private HttpClient BuildNewHttpClient<T>() where T : class
        {
            var builder = new WebHostBuilder();

            var config = new ConfigurationBuilder()
                    .Build();

            var server = new TestServer(builder
                .UseConfiguration(config)
                .UseStartup<T>()
                .UseEnvironment("test"));

            var httpWebClient = server.CreateClient();

            return httpWebClient;
        }
    }
}
namespace FunctionalKanban.Api.Test
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FunctionalKanban.Api.Test.Tools;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Commands;
    using Xunit;

    public class PostCreateTaskShould : BaseTestClass
    {
        [Fact]
        public async Task ReturnHttpOkWhenPostCreateTaskCommand()
        {
            var httpClient = BuildNewHttpClient<DoNothingStartup>();
            var httpResponseMessage = await httpClient
                .PostAsJsonAsync(
                    "task",
                    new CreateTask()
                    {
                        AggregateId =   Guid.NewGuid(),
                        Name =          Guid.NewGuid().ToString(),
                        RemaningWork =  10
                    });

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ReturnHttpBadRequestWhenPostCreateTaskCommandWithNull()
        {
            var httpClient = BuildNewHttpClient<DoNothingStartup>();
            var httpResponseMessage = await httpClient
                .PostAsJsonAsync<object>(
                    "task", null);

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);

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
         
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            responseContent.Should().Be("[\"Les données de la requête ne sont pas serialisables en commande CreateTask\"]");
        }

        [Fact]
        public async Task ReturnHttpBadRequestWhenPostCreateTaskCommandWithIncorrectCommand()
        {
            var httpClient = BuildNewHttpClient<DoNothingStartup>();
            var httpResponseMessage = await httpClient
                .PostAsJsonAsync("task", new CreateTask());

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            responseContent.Should().Be("[\"L'id d'aggregat doit être défini\",\"La tâche dans avoir un nom\"]");
        }

        [Fact]
        public async Task ReturnHttpInternalServerErrorWhenPostSameTask()
        {
            var httpClient = BuildNewHttpClient<InMemoryStartup>();

            var createTaskCommand = new CreateTask()
            {
                AggregateId =   Guid.NewGuid(),
                Name =          Guid.NewGuid().ToString(),
                RemaningWork =  10
            };

            _ = await httpClient
                .PostAsJsonAsync(
                    "task",createTaskCommand);

            var httpResponseMessage = await httpClient
                .PostAsJsonAsync(
                    "task", createTaskCommand);

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            responseContent.Should().Be("\"Un événement pour cette version d'aggregat est déjà présent\"");
        }

        [Fact]
        public async Task AddTaskCreatedEvent()
        { 
            var httpClient = BuildNewHttpClient<InMemoryStartup>();

            var expectedAggregateId = Guid.NewGuid();
            var expectedAggregateName = typeof(TaskEntity).FullName;
            var expectedVersion = 1;

            _ = await httpClient
                .PostAsJsonAsync(
                    "task",
                    new CreateTask()
                    {
                        AggregateId =   expectedAggregateId,
                        Name = Guid.NewGuid().ToString(),
                        RemaningWork =  10
                    });

            var eventLines = InMemoryStartup.DataBase.EventLines.Where(e => e.aggregateId.Equals(expectedAggregateId));

            eventLines.Should().HaveCount(1);

            var eventLine = eventLines.Single();

            eventLine.version.Should().Equals(expectedVersion);
            eventLine.aggregateId.Should().Equals(expectedAggregateId);
            eventLine.aggregateName.Should().Be(expectedAggregateName);
        }

        [Fact]
        public async Task PopulateTaskViewProjection()
        {
            var httpClient = BuildNewHttpClient<InMemoryStartup>();

            var expectedAggregateId = Guid.NewGuid();
            var expectedName = Guid.NewGuid().ToString();
            var expectedStatus = Domain.Task.TaskStatus.Todo;
            var expectedRemaningWork = 10u;

            _ = await httpClient
                .PostAsJsonAsync(
                    "task",
                    new CreateTask()
                    {
                        AggregateId = expectedAggregateId,
                        Name = expectedName,
                        RemaningWork = expectedRemaningWork
                    });

            var taskViewProjections = InMemoryStartup.DataBase.TaskViewProjections.Where(p => p.Id.Equals(expectedAggregateId));

            taskViewProjections.Should().HaveCount(1);

            var taskViewProjection = taskViewProjections.Single();

            taskViewProjection.Name.Should().Be(expectedName);
            taskViewProjection.Status.Should().Be(expectedStatus);
            taskViewProjection.RemaningWork.Should().Be(expectedRemaningWork);
        }
    }
}
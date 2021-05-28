namespace FunctionalKanban.Web.Api.Test
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FunctionalKanban.Web.Api.Test.Tools;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Domain.Task.Events;
    using FunctionalKanban.Domain.ViewProjections;
    using FunctionalKanban.Infrastructure.InMemory;
    using Xunit;

    public class PostCreateTaskShould : BaseTestClass
    {
        [Fact]
        public async Task ReturnHttpOkWhenPostCreateTaskCommand()
        {
            var httpClient = BuildNewHttpClient<DoNothingStartup>(
                new InMemoryDatabase(), new InMemoryDatabase());

            var httpResponseMessage = await httpClient
                .PostAsJsonAsync(
                    "task",
                    new CreateTask()
                    {
                        EntityId =   Guid.NewGuid(),
                        Name =          Guid.NewGuid().ToString(),
                        RemaningWork =  10
                    });

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ReturnHttpBadRequestWhenPostCreateTaskCommandWithNull()
        {
            var httpClient = BuildNewHttpClient<DoNothingStartup>(
                new InMemoryDatabase(), new InMemoryDatabase());

            var httpResponseMessage = await httpClient
                .PostAsJsonAsync<object>(
                    "task", null);

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            responseContent.Should().Be("[\"Données incomplètes\"]");
        } 

        [Fact]
        public async Task ReturnHttpBadRequestWhenPostCreateTaskCommandWithWrongCommand()
        {
            var httpClient = BuildNewHttpClient<DoNothingStartup>(
                new InMemoryDatabase(), new InMemoryDatabase());
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
            var httpClient = BuildNewHttpClient<DoNothingStartup>
                (new InMemoryDatabase(), new InMemoryDatabase());
            var httpResponseMessage = await httpClient
                .PostAsJsonAsync("task", new CreateTask());

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            responseContent.Should().Be("[\"L'id d'aggregat doit être défini\",\"La tâche dois avoir un nom\"]");
        }

        [Fact]
        public async Task ReturnHttpInternalServerErrorWhenPostSameTask()
        {
            var httpClient = BuildNewHttpClient<InMemoryStartup>(
                new InMemoryDatabase(), new InMemoryDatabase());

            var createTaskCommand = new CreateTask()
            {
                EntityId =   Guid.NewGuid(),
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
            responseContent.Should().Be("\"Un événement pour cette version d'entité est déjà présent\"");
        }

        [Fact]
        public async Task AddTaskCreatedEvent()
        {
            var eventDataBase = new InMemoryDatabase();
            var httpClient = BuildNewHttpClient<InMemoryStartup>(eventDataBase, new InMemoryDatabase());

            var expectedEntityId = Guid.NewGuid();
            var expectedEntityName = typeof(TaskEntityState).FullName;
            var expectedVersion = 1;

            _ = await httpClient
                .PostAsJsonAsync(
                    "task",
                    new CreateTask()
                    {
                        EntityId =   expectedEntityId,
                        Name = Guid.NewGuid().ToString(),
                        RemaningWork =  10
                    });

            var events = eventDataBase.EventsByEntityId(expectedEntityId);

            events.Should().HaveCount(1);

            var eventLine = events.Single();

            eventLine.EntityVersion.Should().Equals(expectedVersion);
            eventLine.EntityId.Should().Equals(expectedEntityId);
            eventLine.EntityName.Should().Be(expectedEntityName);

            eventLine.Should().BeOfType<TaskCreated>();
        }

        [Fact]
        public async Task PopulateTaskViewProjection()
        {
            var viewProjectionDataBase = new InMemoryDatabase();
            var httpClient = BuildNewHttpClient<InMemoryStartup>(
                new InMemoryDatabase(), viewProjectionDataBase);

            var expectedEntityId = Guid.NewGuid();
            var expectedEntityName = Guid.NewGuid().ToString();
            var expectedStatus = Domain.Task.TaskStatus.Todo;
            var expectedRemaningWork = 10u;

            _ = await httpClient
                .PostAsJsonAsync(
                    "task",
                    new CreateTask()
                    {
                        EntityId = expectedEntityId,
                        Name = expectedEntityName,
                        RemaningWork = expectedRemaningWork
                    });

            var taskViewProjections = viewProjectionDataBase.GetProjections<TaskViewProjection>().Where(p => p.Id.Equals(expectedEntityId));

            taskViewProjections.Should().HaveCount(1);

            var taskViewProjection = taskViewProjections.Single();

            taskViewProjection.Name.Should().Be(expectedEntityName);
            taskViewProjection.Status.Should().Be(expectedStatus);
            taskViewProjection.RemaningWork.Should().Be(expectedRemaningWork);
        }
    }
}
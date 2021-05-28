namespace FunctionalKanban.Web.Api.Test
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FunctionalKanban.Web.Api.Test.Tools;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Infrastructure.InMemory;
    using Xunit;

    public class PostChangeTaskStatusShould : BaseTestClass
    {
        [Fact]
        public async Task ReturnHttpOkWhenPostChangeTaskStatus()
        {
            var entityId = Guid.NewGuid();
            var eventDataBase = new InMemoryDatabase();

            var httpClient = BuildNewHttpClient<InMemoryStartup>(eventDataBase, new InMemoryDatabase());

            await InitNewTask(httpClient, entityId);
            
            var httpResponseMessage = await httpClient
                .PostAsJsonAsync(
                    "task/changeStatus",
                    new ChangeTaskStatus()
                    {
                        EntityId = entityId,
                        TaskStatus = Domain.Task.TaskStatus.InProgress
                    }); 

            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            var lines = eventDataBase.EventsByEntityId(entityId);

            lines.Should().HaveCount(2);
            lines.FirstOrDefault(e => e.EntityVersion.Equals(2)).Should().NotBeNull();
        }
    }
}

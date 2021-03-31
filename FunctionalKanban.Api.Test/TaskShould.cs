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

    public class TaskShould
    {
        [Fact]
        public async Task ReturnHttpCreatedWhenPostCreateTaskCommand()
        {
            var httpClient = BuildNewHttpClient<Startup>();
            var httpResponseMessage = await httpClient
                .PostAsJsonAsync(
                    "task",
                    new CreateTask()
                    {
                        EntityId = Guid.NewGuid(),
                        Name = Guid.NewGuid().ToString(),
                        RemaningWork = 10,
                        TimeStamp = DateTime.Now
                    });

            httpResponseMessage.StatusCode.Should().Equals(HttpStatusCode.Created);
        }

        [Fact]
        public async Task ReturnHttpBadRequestWhenPostCreateTaskCommandWithIncorrectCommand()
        {
            var httpClient = BuildNewHttpClient<Startup>();
            var httpResponseMessage = await httpClient
                .PostAsJsonAsync(
                    "task", Guid.NewGuid());

            httpResponseMessage.StatusCode.Should().Equals(HttpStatusCode.BadRequest);
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
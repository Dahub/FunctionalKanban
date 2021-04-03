namespace FunctionalKanban.Api.Test.Tools
{
    using System.Net.Http;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;

    public abstract class BaseTestClass
    {
        protected HttpClient BuildNewHttpClient<T>() where T : class
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

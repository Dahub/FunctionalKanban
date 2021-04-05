namespace FunctionalKanban.Api.Test.Tools
{
    using System;
    using System.Net.Http;
    using FunctionalKanban.Infrastructure.InMemory;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;

    public abstract class BaseTestClass
    {
        protected static HttpClient BuildNewHttpClient<T>(InMemoryDatabase dataBase) where T : class, ITestStartup
        {
            var builder = new WebHostBuilder();

            var config = new ConfigurationBuilder()
                    .Build();

            var server = new TestServer(builder
                .UseConfiguration(config)
                .UseStartup((w) => BuildTestStartup<T>(dataBase, w))
                .UseEnvironment("test"));

            var httpWebClient = server.CreateClient();

            return httpWebClient;
        }

        private static ITestStartup BuildTestStartup<T>(InMemoryDatabase dataBase, WebHostBuilderContext context)
        {
            var startup = (ITestStartup)Activator.CreateInstance(typeof(T), new object[] { context.Configuration });
            startup.DataBase = dataBase;
            return startup;
        }
    }
}

namespace FunctionalKanban.Api.Test
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Unit = System.ValueTuple;

    public class InMemoryStartup : Startup
    {
        public static IEventStream EventStream { get; set; }

        public InMemoryStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override Func<Event, Exceptional<Unit>> PublishEventMethod(IServiceCollection services) =>
            (evt) => new EventBus(EventStream).Publish(evt);
    }
}

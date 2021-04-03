namespace FunctionalKanban.Api.Test.Tools
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure;
    using FunctionalKanban.Infrastructure.Abstraction;
    using FunctionalKanban.Infrastructure.InMemory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Unit = System.ValueTuple;

    internal class InMemoryStartup : Startup
    {
        public static IEventStream EventStream { get; set; }

        public InMemoryStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override Func<Guid, Option<State>> GetEntityMethod(IServiceCollection services) =>
            (id) => new InMemoryEntityStateRepository().GetById(id);

        protected override Func<Event, Exceptional<Unit>> PublishEventMethod(IServiceCollection services) =>
            (evt) => new EventBus(EventStream??new InMemoryEventStream()).Publish(evt);
    }
}

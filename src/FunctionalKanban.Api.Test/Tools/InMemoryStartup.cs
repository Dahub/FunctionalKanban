namespace FunctionalKanban.Api.Test.Tools
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure;
    using FunctionalKanban.Infrastructure.InMemory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Unit = System.ValueTuple;

    internal class InMemoryStartup : Startup, ITestStartup
    {
        public InMemoryDatabase DataBase { get; set; }

        public InMemoryStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override Func<Guid, Option<State>> GetEntityMethod(IServiceCollection services) =>
            (id) => new InMemoryEntityStateRepository(DataBase).GetById(id);

        protected override Func<Event, Exceptional<Unit>> PublishEventMethod(IServiceCollection services) =>
            (evt) => new EventBus(
                new InMemoryEventStream(DataBase),
                new Notifier(new InMemoryViewProjectionRepository<TaskViewProjection>(DataBase))
                ).Publish(evt);

        protected override InMemoryDatabase BuildDataBase() => DataBase;
    }
}

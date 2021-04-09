namespace FunctionalKanban.Api.Test.Tools
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure;
    using FunctionalKanban.Infrastructure.Abstraction;
    using FunctionalKanban.Infrastructure.InMemory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Unit = System.ValueTuple;

    internal class InMemoryStartup : Startup, ITestStartup
    {
        public InMemoryDatabase ViewProjectionDataBase { get; set; }

        public InMemoryDatabase EventDataBase { get; set; }

        public InMemoryStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override Func<Guid, Exceptional<Option<State>>> GetEntityMethod(IServiceCollection services) =>
            (id) => new EntityStateRepository(EventDataBase).GetById(id);

        protected override Func<Event, Exceptional<Unit>> PublishEventMethod(IServiceCollection services) =>
            (evt) => new EventBus(
                new EventStream(EventDataBase),
                new Notifier(new ViewProjectionRepository<TaskViewProjection>(ViewProjectionDataBase))
                ).Publish(evt);

        protected override IViewProjectionDataBase BuildViewProjectionDataBase() => ViewProjectionDataBase;

        protected override IEventDataBase BuildEventDataBase() => EventDataBase;
    }
}

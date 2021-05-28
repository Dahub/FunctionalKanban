namespace FunctionalKanban.Web.Api.Test.Tools
{
    using System;
    using FunctionalKanban.Domain.Common;
    using LaYumba.Functional;
    using FunctionalKanban.Infrastructure.InMemory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using static LaYumba.Functional.F;
    using Unit = System.ValueTuple;
    using FunctionalKanban.Infrastructure.Abstraction;

    internal class DoNothingStartup : Startup, ITestStartup
    {
        public InMemoryDatabase ViewProjectionDataBase { get; set; }

        public InMemoryDatabase EventDataBase { get; set; }

        public DoNothingStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override IDatabaseFactory GetDatabaseFactory() => new InMemoryDatabaseFactory();

        protected override Func<Guid, Exceptional<Option<State>>> GetEntityMethod(IServiceCollection services) => (id) => (Option<State>)None;

        protected override Func<Event, Exceptional<Unit>> PublishEventMethod(IServiceCollection services) => (evt) => Unit.Create();
    }
}

namespace FunctionalKanban.Api.Test.Tools
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.InMemory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using static FunctionalKanban.Functional.F;
    using Unit = System.ValueTuple;

    internal class DoNothingStartup : Startup, ITestStartup
    {
        public InMemoryDatabase DataBase { get; set; }

        public DoNothingStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override Func<Guid, Exceptional<Option<State>>> GetEntityMethod(IServiceCollection services) => (id) => (Option<State>)None;

        protected override Func<Event, Exceptional<Unit>> PublishEventMethod(IServiceCollection services) => (evt) => Unit.Create();
    }
}

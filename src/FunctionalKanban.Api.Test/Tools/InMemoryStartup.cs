namespace FunctionalKanban.Api.Test.Tools
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using LaYumba.Functional;
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

        protected override IViewProjectionDataBase BuildViewProjectionDataBase() => ViewProjectionDataBase;

        protected override IEventDataBase BuildEventDataBase() => EventDataBase;
    }
}

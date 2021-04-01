﻿namespace FunctionalKanban.Api.Test
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using static FunctionalKanban.Functional.F;
    using Unit = System.ValueTuple;

    internal class DoNothingStartup : Startup
    {
        public DoNothingStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override Func<Guid, Option<State>> GetEntityMethod => (id) => None;

        protected override Func<Event, Unit> PublishEventMethod(IServiceCollection services) => (evt) => Unit.Create();
    }
}

namespace FunctionalKanban.Api
{
    using System;
    using FunctionalKanban.Application;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using static FunctionalKanban.Functional.F;
    using Unit = System.ValueTuple;

    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IEventStream, InMemoryEventStream>();
            services.AddTransient<IEventBus, EventBus>();

            services.AddRouting();

            services.AddTransient(s => new CommandHandler(
                getEntity:      GetEntityMethod,
                publishEvent:   PublishEventMethod(services)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/task", async context => await context.ExecuteCommand<CreateTask>());
            });
        }

        protected virtual Func<Guid, Option<State>> GetEntityMethod => (id) => None;

        protected virtual Func<Event, Exceptional<Unit>> PublishEventMethod(IServiceCollection services) =>
            (evt) => services.BuildServiceProvider().GetRequiredService<IEventBus>().Publish(evt);
    }    
}

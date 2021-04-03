namespace FunctionalKanban.Api
{
    using System;
    using FunctionalKanban.Application;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure;
    using FunctionalKanban.Infrastructure.Abstraction;
    using FunctionalKanban.Infrastructure.InMemory;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Unit = System.ValueTuple;

    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IEntityStateRepository, InMemoryEntityStateRepository>();
            services.AddTransient<IEventStream, InMemoryEventStream>();
            services.AddTransient<IEventBus, EventBus>();

            services.AddRouting();

            services.AddTransient(s => new CommandHandler(
                getEntity: GetEntityMethod(services),
                publishEvent: PublishEventMethod(services)));
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
                endpoints.MapPost("/task/changeStatus", async context => await context.ExecuteCommand<ChangeTaskStatus>());
            });
        }

        protected virtual Func<Guid, Option<State>> GetEntityMethod(IServiceCollection services) =>
            (id) => services.BuildServiceProvider().GetRequiredService<IEntityStateRepository>().GetById(id);

        protected virtual Func<Event, Exceptional<Unit>> PublishEventMethod(IServiceCollection services) =>
            (evt) => services.BuildServiceProvider().GetRequiredService<IEventBus>().Publish(evt);
    }
}

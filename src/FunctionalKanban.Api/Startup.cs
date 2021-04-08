namespace FunctionalKanban.Api
{
    using System;
    using FunctionalKanban.Application;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Domain.Task.Queries;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure;
    using FunctionalKanban.Infrastructure.Abstraction;
    using FunctionalKanban.Infrastructure.InMemory;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
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
            services.AddSingleton<IInMemoryDatabase>(BuildDataBase());
            services.AddScoped<IEventStream, InMemoryEventStream>();

            services.AddScoped<IEntityStateRepository, InMemoryEntityStateRepository>();
            services.AddScoped<IViewProjectionRepository<TaskViewProjection>, InMemoryViewProjectionRepository<TaskViewProjection>>();

            services.AddScoped<INotifier, Notifier>();
            services.AddScoped<IEventBus, EventBus>();
            
            services.AddScoped(s => new CommandHandler(
                getEntity: GetEntityMethod(services),
                publishEvent: PublishEventMethod(services)));

            services.AddRouting();
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
                endpoints.MapGet("/", async context => await context.Response.WriteAsync("Hello world !"));
                endpoints.MapGet("/task", async context => await context.ExecuteQuery<GetTaskQuery, TaskViewProjection>());

                endpoints.MapPost("/task", async context => await context.ExecuteCommand<CreateTask>());
                endpoints.MapPost("/task/changeStatus", async context => await context.ExecuteCommand<ChangeTaskStatus>());
            });
        }

        protected virtual Func<Guid, Exceptional<Option<State>>> GetEntityMethod(IServiceCollection services) =>
            (id) => services.BuildServiceProvider().GetRequiredService<IEntityStateRepository>().GetById(id);

        protected virtual Func<Event, Exceptional<Unit>> PublishEventMethod(IServiceCollection services) =>
            (evt) => services.BuildServiceProvider().GetRequiredService<IEventBus>().Publish(evt);

        protected virtual InMemoryDatabase BuildDataBase() => new InMemoryDatabase();
    }
}

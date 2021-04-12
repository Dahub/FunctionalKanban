namespace FunctionalKanban.Api
{
    using System;
    using FunctionalKanban.Application.Commands;
    using FunctionalKanban.Application.Dtos;
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
            services.AddSingleton(BuildViewProjectionDataBase());
            services.AddSingleton(BuildEventDataBase());
            services.AddScoped<IEventStream, EventStream>();

            services.AddScoped<IEntityStateRepository, EntityStateRepository>();
            services.AddScoped<IViewProjectionRepository<TaskViewProjection>, ViewProjectionRepository<TaskViewProjection>>();

            services.AddScoped<INotifier, Notifier>();
            services.AddScoped<IEventBus>(e => new EventBus(
                streamEvent: StreamEventMethod(services),
                notifySubscribers: NotitySubscribersMethod(services)));

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
                endpoints.MapGet("/task", async context => await context.ExecuteQuery<GetTaskQuery, TaskViewProjection, TaskDto>());
                endpoints.MapGet("/task/{id:guid}", async context => await context.ExecuteQuery<GetTaskByIdQuery, TaskViewProjection, TaskDto>());
                
                endpoints.MapPost("/task", async context => await context.ExecuteCommand<CreateTask>());
                endpoints.MapPost("/task/changeStatus", async context => await context.ExecuteCommand<ChangeTaskStatus>());
                endpoints.MapPost("/task/delete", async context => await context.ExecuteCommand<DeleteTask>());
            });
        }

        protected virtual Func<Guid, Exceptional<Option<State>>> GetEntityMethod(IServiceCollection services) =>
            (id) => GetService<IEntityStateRepository>(services).GetById(id);

        protected virtual Func<Event, Exceptional<Unit>> PublishEventMethod(IServiceCollection services) =>
            (evt) => GetService<IEventBus>(services).Publish(evt);

        protected virtual Func<Event, Exceptional<Unit>> NotitySubscribersMethod(IServiceCollection services) =>
            (evt) => GetService<INotifier>(services).Notity(evt);

        protected virtual Func<Event, Exceptional<Unit>> StreamEventMethod(IServiceCollection services) =>
            (evt) => GetService<IEventStream>(services).Push(evt);

        protected virtual IViewProjectionDataBase BuildViewProjectionDataBase() => new InMemoryDatabase();

        protected virtual IEventDataBase BuildEventDataBase() => new InMemoryDatabase();

        private static T GetService<T>(IServiceCollection services) where T : notnull => services.BuildServiceProvider().GetRequiredService<T>();
    }
}

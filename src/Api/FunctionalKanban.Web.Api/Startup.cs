namespace FunctionalKanban.Web.Api
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Application.Commands;
    using FunctionalKanban.Application.Queries;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Core.Domain.Project.Commands;
    using FunctionalKanban.Core.Domain.Project.Queries;
    using FunctionalKanban.Core.Domain.Task.Commands;
    using FunctionalKanban.Core.Domain.Task.Queries;
    using FunctionalKanban.Infrastructure.Implementation;
    using FunctionalKanban.Infrastructure.Abstraction;
    using FunctionalKanban.Infrastructure.SqlServer;
    using LaYumba.Functional;
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
            services.AddScoped(s => GetDatabaseFactory().GetEventDatabase());
            services.AddScoped(s => GetDatabaseFactory().GetViewProjectionDatabase());
            services.AddScoped<IEventStream, EventStream>();

            services.AddScoped<IEntityStateRepository, EntityStateRepository>();
            services.AddScoped<IViewProjectionRepository, ViewProjectionRepository>();

            services.AddScoped<INotifier, ViewProjectionNotifier>();
            services.AddScoped<IEventBus>(e => new EventBus(
                streamEvent: StreamEventMethod(services),
                notifySubscribers: NotitySubscribersMethod(services)));

            services.AddScoped(s => new CommandHandler(
                getEntity: GetEntityMethod(services),
                publishEvent: PublishEventMethod(services)));

            services.AddScoped(s => new QueryHandler(
                findProjections: GetFindProjectionsMethod(services)));

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
            app.UseMiddleware<EndRequestMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => await context.Response.WriteAsync("Hello world !"));
                endpoints.MapGet("/task", async context => await context.ExecuteQuery<GetTaskQuery>());
                endpoints.MapGet("/task/{id:guid}", async context => await context.ExecuteQuery<GetTaskByIdQuery>());
                endpoints.MapGet("/project/{id:guid}", async context => await context.ExecuteQuery<GetProjectByIdQuery>());

                endpoints.MapPost("/task", async context => await context.ExecuteCommand<CreateTask>());
                endpoints.MapPost("/task/changeStatus", async context => await context.ExecuteCommand<ChangeTaskStatus>());
                endpoints.MapPost("/task/delete", async context => await context.ExecuteCommand<DeleteTask>());
                endpoints.MapPost("/task/changeRemaningWork", async context => await context.ExecuteCommand<ChangeRemaningWork>());
                endpoints.MapPost("/task/linkToProject", async context => await context.ExecuteCommand<LinkToProject>());
                endpoints.MapPost("/project", async context => await context.ExecuteCommand<CreateProject>());                
                endpoints.MapPost("/project/delete", async context => await context.ExecuteCommand<DeleteProject>());
            });
        }

        protected virtual IDatabaseFactory GetDatabaseFactory()
          => new SqlServerEfContextFactory(
              Configuration.GetConnectionString("EventDatabaseConnexionString"),
              Configuration.GetConnectionString("ViewProjectionDatabaseConnexionString"));

        protected virtual Func<Guid, Exceptional<Option<State>>> GetEntityMethod(IServiceCollection services) =>
            (id) => GetService<IEntityStateRepository>(services).GetById(id);

        protected virtual Func<Event, Exceptional<Unit>> PublishEventMethod(IServiceCollection services) =>
            (evt) => GetService<IEventBus>(services).Publish(evt);

        protected virtual Func<Event, Exceptional<Unit>> NotitySubscribersMethod(IServiceCollection services) =>
            (evt) => GetService<INotifier>(services).Notify(evt);

        protected virtual Func<Event, Exceptional<Unit>> StreamEventMethod(IServiceCollection services) =>
            (evt) => GetService<IEventStream>(services).Push(evt);

        protected virtual Func<Type, Func<ViewProjection, bool>, Exceptional<IEnumerable<ViewProjection>>> GetFindProjectionsMethod(IServiceCollection services) =>
             GetService<IViewProjectionRepository>(services).Get;

        private static T GetService<T>(IServiceCollection services) where T : notnull => services.BuildServiceProvider().GetRequiredService<T>();
    }
}

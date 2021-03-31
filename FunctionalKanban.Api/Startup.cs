namespace FunctionalKanban.Api
{
    using FunctionalKanban.Application;
    using FunctionalKanban.Domain.Task.Commands;
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
            services.AddRouting();

            services.AddTransient(s => new CommandHandler(
                (id) => None,
                (evt) => Unit.Create()));
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
    }    
}

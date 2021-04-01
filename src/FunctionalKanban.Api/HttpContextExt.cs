namespace FunctionalKanban.Api
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using FunctionalKanban.Application;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using static FunctionalKanban.Functional.F;

    internal static class HttpContextExt
    {
        public static async Task ExecuteCommand<T>(this HttpContext context) where T : Command =>
             (await context.ReadCommandAsync<T>())
                        .Bind(CommandValidator.Validate)
                        .Bind(c => context.GetCommandHandler().Handle(c))
                        .Match(
                            async (errors) => { await context.SetResponseBadRequest(errors); },
                            (_) => { context.SetResponseCreated(); return; });

        private static async Task SetResponseBadRequest(this HttpContext context, IEnumerable<Error> errors)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(errors.Map(e => e.Message));
        }

        private static void SetResponseCreated(this HttpContext context) => context.Response.StatusCode = (int)HttpStatusCode.Created;

        private static CommandHandler GetCommandHandler(this HttpContext context) => context.RequestServices.GetService<CommandHandler>();

        private static async Task<Validation<T>> ReadCommandAsync<T>(this HttpContext context) where T : Command
        {
            try
            {
                return await context.Request.ReadFromJsonAsync<T>();
            }
            catch
            {
                return Invalid(Error($"Les données de la requête ne sont pas serialisables en commande {typeof(T).Name}"));
            }
        }
    }
}

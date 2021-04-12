namespace FunctionalKanban.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using FunctionalKanban.Application.Commands;
    using FunctionalKanban.Application.Dtos;
    using FunctionalKanban.Application.Queries;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using static FunctionalKanban.Functional.F;
    using Unit = System.ValueTuple;

    internal static class HttpContextExt
    {
        public static async Task ExecuteCommand<T>(this HttpContext context) where T : Command =>
            (await context.ReadCommandAsync<T>()).
                Bind(HandleWithCommandHandler(context)).
                Match(
                    Invalid:    async (errors)  => await context.SetResponseBadRequest(errors),
                    Valid:      (v)             =>
                    {
                        v.Match(
                            Exception:  async (ex)  => await context.SetResponseInternalServerError(ex),
                            Success:    _           => { context.SetResponseOk(); return; }
                        );
                    });

        public static async Task ExecuteQuery<TQuery, TDto>(this HttpContext context) 
                where TQuery : Query, new()
                where TDto : Dto =>
            await context.ExtractParameters().
                Bind(HandleWithQueryHandler<TQuery, TDto>(context)).
                Match(
                    Exception:  (ex)    => context.SetResponseInternalServerError(ex),
                    Success:    (v)     => context.SetResponseOk(v));

        private static Func<Dictionary<string, string>, Exceptional<IEnumerable<TDto>>> HandleWithQueryHandler<TQuery, TDto>(HttpContext context)
                where TQuery : Query, new()
                where TDto : Dto =>
            parameters =>
            {
                var service = context.RequestServices.GetService<QueryHandler>();
                return service == null
                    ? new Exception("Impossible de charger le queryHandler")
                    : service.Handle<TQuery, TDto>(parameters);
            };

        private static Func<Command, Validation<Exceptional<Unit>>> HandleWithCommandHandler(HttpContext context) =>
            c =>
            {
                var service = context.RequestServices.GetService<CommandHandler>();
                return service == null
                    ? Invalid("Impossible de charger le commandHandler")
                    : service.Handle(c);
            };

        private static async Task<Validation<T>> ReadCommandAsync<T>(this HttpContext context) where T : Command =>
            (await RunAsync(() => context.Request.ReadFromJsonAsync<T>())).
                Match(
                    Exception:  (ex)    => Invalid($"Les données de la requête ne sont pas serialisables en commande {typeof(T).Name}"),
                    Success:    (value) => value == null
                                            ? Invalid("Données incomplètes")
                                            : Valid(value));

        private static async Task<Exceptional<T?>> RunAsync<T>(Func<ValueTask<T?>> asyncMethod) where T : Command
        {
            try { return await asyncMethod(); }
            catch(Exception ex) { return ex; }
        }

        private static Exceptional<Dictionary<string, string>> ExtractParameters(this HttpContext context) =>
            Try(() =>
                context.Request.Query.Select(v => KeyValuePair.Create(v.Key, (string)v.Value)).
                Union(context.Request.RouteValues.Select(v => KeyValuePair.Create(v.Key, (string)v.Value))).
                ToDictionary((kv) => kv.Key, (kv) => kv.Value)).
            Run();

        private static async Task SetResponseBadRequest(this HttpContext context, IEnumerable<Error> errors)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(errors.Map(e => e.Message));
        }

        private static async Task SetResponseInternalServerError(this HttpContext context, Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(ex.Message);
        }

        private static void SetResponseOk(this HttpContext context) => 
            context.Response.StatusCode = (int)HttpStatusCode.OK;

        private static async Task SetResponseOk<TValue>(this HttpContext context, TValue value)
        {
            context.SetResponseOk();
            await context.Response.WriteAsJsonAsync(value);
        }
    }
}

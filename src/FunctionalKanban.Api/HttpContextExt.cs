namespace FunctionalKanban.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using FunctionalKanban.Application;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using static FunctionalKanban.Application.QueryBuilder;
    using static FunctionalKanban.Functional.F;
    using Unit = System.ValueTuple;

    internal static class HttpContextExt
    {
        public static async Task ExecuteCommand<T>(this HttpContext context) where T : Command =>
            (await context.ReadCommandAsync<T>())
                .Bind(HandleWithCommandHandler(context))
                .Match(
                    Invalid:    async (errors)  => await context.SetResponseBadRequest(errors),
                    Valid:      (v)             =>
                    {
                        v.Match(
                            Exception:  async (ex)  => await context.SetResponseInternalServerError(ex),
                            Success:    _           => { context.SetResponseOk(); return; }
                        );
                    });

        public static async Task ExecuteQuery<T>(this HttpContext context) where T : ViewProjection =>
            await context.ExtractParameters()
                .Bind(BuildQuery<T>)
                .Bind(BuildRepository<T>(context))
                .Bind(LaunchQuery<T>())
                .Match(
                    Exception:  (ex)    => context.SetResponseInternalServerError(ex),
                    Success:    (v)     => context.SetResponseOk(v.Map(p => (T)p)));

        private static Func<(Query, IViewProjectionRepository<T>), Exceptional<IEnumerable<ViewProjection>>> LaunchQuery<T>() where T : ViewProjection =>
            tuple => tuple.Item2.Get(tuple.Item1.BuildPredicate());

        private static Func<Query, Exceptional<(Query, IViewProjectionRepository<T>)>> BuildRepository<T>(HttpContext context) where T : ViewProjection =>
            query => (query, context.RequestServices.GetService<IViewProjectionRepository<T>>());

        private static Func<Command, Validation<Exceptional<Unit>>> HandleWithCommandHandler(HttpContext context) =>
            c => context.RequestServices.GetService<CommandHandler>().Handle(c);

        private static async Task<Validation<T>> ReadCommandAsync<T>(this HttpContext context) where T : Command =>
            (await RunAsync(() => context.Request.ReadFromJsonAsync<T>())).Match(
                    Exception:  (ex)    => Invalid($"Les données de la requête ne sont pas serialisables en commande {typeof(T).Name}"),
                    Success:    (value) => Valid(value));

        private static async Task<Exceptional<T>> RunAsync<T>(Func<ValueTask<T>> asyncMethod) where T : Command
        {
            try { return await asyncMethod(); }
            catch(Exception ex) { return ex; }
        }

        private static Exceptional<Dictionary<string, string>> ExtractParameters(this HttpContext context) => 
            Try(() => context.Request.Query.Keys.ToDictionary((k) => k, (k) => context.Request.Query.TryGetValueOrException(k))).Run();

        private static string TryGetValueOrException(this IQueryCollection parameters, string key) =>
            parameters.TryGetValue(key, out var value)
            ? (string)value
            : throw new Exception($"Impossible de lire le paramètre {key}");

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

        private static void SetResponseOk(this HttpContext context) => context.Response.StatusCode = (int)HttpStatusCode.OK;

        private static async Task SetResponseOk<TValue>(this HttpContext context, TValue value)
        {
            context.SetResponseOk();
            await context.Response.WriteAsJsonAsync(value);
        }
    }
}

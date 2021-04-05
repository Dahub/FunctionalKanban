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
    using static FunctionalKanban.Functional.F;
    using static FunctionalKanban.Application.CommandValidator;
    using static FunctionalKanban.Application.QueryBuilder;

    internal static class HttpContextExt
    {
        public static async Task ExecuteCommand<T>(this HttpContext context) where T : Command =>
            (await context.ReadCommandAsync<T>())
                       .Bind(Validate)
                       .Bind(HandleWithCommandHandler(context))
                       .Match(
                           Invalid: async (errors)  => await context.SetResponseBadRequest(errors),
                           Valid:   (v)             =>
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
                .Run()
                .Match(
                    Exception: (ex)     => context.SetResponseInternalServerError(ex),
                    Success: (v)        => context.SetResponseOk(v.Map(p => (T)p)));

        private static Func<(Query, IViewProjectionRepository<T>), Try<IEnumerable<ViewProjection>>> LaunchQuery<T>() where T : ViewProjection =>
            tuple => tuple.Item2.Get(tuple.Item1.BuildPredicate());
        
        private static Func<Query, Try<(Query, IViewProjectionRepository<T>)>> BuildRepository<T>(HttpContext context) where T : ViewProjection =>
            query => Try(() => (query, context.RequestServices.GetService<IViewProjectionRepository<T>>()));

        private static Func<Command, Validation<Exceptional<ValueTuple>>> HandleWithCommandHandler(HttpContext context) =>
            c => context.RequestServices.GetService<CommandHandler>().Handle(c);

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

        private static Try<Dictionary<string, string>> ExtractParameters(this HttpContext context)
        {
            return Try(() => context.Request.Query.Keys.ToDictionary((k) => k, (k) => GetValue(k, context.Request.Query)));
            string GetValue(string key, IQueryCollection parameters)
            {
                if (parameters.TryGetValue(key, out var value))
                {
                    return value;
                }
                throw new Exception($"Impossible de lire le paramètre {key}");
            }
        }

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

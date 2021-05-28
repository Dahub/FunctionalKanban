namespace FunctionalKanban.Web.Api
{
    using System.Threading.Tasks;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Microsoft.AspNetCore.Http;

    public class EndRequestMiddleware
    {
        private readonly RequestDelegate _next;

        public EndRequestMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context, IEventDataBase dataBase)
        {
            await _next(context);

            var task = context.Response.StatusCode switch
            {
                >= 200 and < 400    => dataBase.Commit(),
                _                   => dataBase.Rollback()
            };

            await task;
        }
    }
}

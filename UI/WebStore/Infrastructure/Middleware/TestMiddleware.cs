using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebStore.Infrastructure.Middleware
{
    public class TestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TestMiddleware> _logger;

        public TestMiddleware(RequestDelegate next, ILogger<TestMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //await context.Response.WriteAsJsonAsync(_logger);  если что-то записали в респонз - после этого нельзя вызывать _next
            //await context.Response.WriteAsJsonAsync<ILogger>(null);
            
            //предобработка

            var task = _next(context);  //запуск следующего узла конвеера

            //параллельная обработка

            await task;  //ожидание завершения обработки на конвейере и возврата управления обратно

            //постобработка  (здесь можно писать в респонз)
        }
    }
}

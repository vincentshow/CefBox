using CefBox.Extensions;
using CefBox.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CefBox.Middlewares
{
    public class LoggerMiddleware : IMiddleware
    {
        private readonly ILogger _logger;
        
        public LoggerMiddleware(ILoggerFactory factory)
        {
            _logger = factory.CreateLogger(this.GetType());
        }

        public async Task InvokeAsync(AppRequest context, RequestDelegate next)
        {
            _logger.Info($"receive request {context.Id} in tid_{Thread.CurrentThread.ManagedThreadId} with param:{context.ToLogString()}");

            var watch = Stopwatch.StartNew();
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.Error($"error in middleware: {ex.Message}", ex);
                await context.ExecJSCallback(() => { throw ex; }).ConfigureAwait(false);
            }
            finally
            {
                watch.Stop();
                _logger.Info($"finished request {context.Id} in tid_{Thread.CurrentThread.ManagedThreadId} cost {watch.ElapsedMilliseconds}ms");
            }
        }
    }
}

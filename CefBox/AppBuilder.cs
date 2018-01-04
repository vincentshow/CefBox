using CefBox.Middlewares;
using CefBox.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefBox
{
    public delegate Task RequestDelegate(AppRequest request);

    public class AppBuilder
    {
        private readonly IList<Func<RequestDelegate, RequestDelegate>> _components = new List<Func<RequestDelegate, RequestDelegate>>();

        public AppBuilder(IServiceProvider serviceProvider = null)
        {
        }

        public AppBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            _components.Add(middleware);
            return this;
        }

        public RequestDelegate Build()
        {
            RequestDelegate app = context => context.ExecJSCallback(() => { throw new NotImplementedException(nameof(AppBuilder)); });

            foreach (var component in _components.Reverse())
            {
                app = component(app);
            }

            return app;
        }

        public AppBuilder Use(Func<AppRequest, Func<Task>, Task> middleware)
        {
            return this.Use(next =>
            {
                return context =>
                {
                    Func<Task> simpleNext = () => next(context);
                    return middleware(context, simpleNext);
                };
            });
        }

        public AppBuilder UseMiddleware<TMiddleWare>(Func<TMiddleWare> getInstance) where TMiddleWare : IMiddleware
        {
            return this.Use(next =>
            {
                IMiddleware middleware = getInstance.Invoke();

                if (middleware == null)
                {
                    throw new InvalidOperationException($"cannot get instance of type {typeof(TMiddleWare).ToString()}");
                }

                return async context =>
                {
                    await middleware.InvokeAsync(context, next);
                };
            });
        }
    }
}

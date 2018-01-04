using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefBox.Middlewares
{
    public interface IMiddleware
    {
        /// <summary>
        /// Request handling method.
        /// </summary>
        /// <param name="context">The CefRequest for the current request.</param>
        /// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
        /// <returns>A <see cref="Task"/> that represents the execution of this middleware.</returns>
        Task InvokeAsync(AppRequest context, RequestDelegate next);
    }
}

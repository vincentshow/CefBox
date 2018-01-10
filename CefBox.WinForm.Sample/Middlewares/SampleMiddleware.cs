using CefBox.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefBox.Models;
using System.Windows.Forms;

namespace CefBox.WinForm.Sample.Middlewares
{
    /// <summary>
    /// nothing to do
    /// </summary>
    public class SampleMiddleware : IMiddleware
    {
        public async Task InvokeAsync(AppRequest context, RequestDelegate next)
        {
            if (context.Service?.ToLower() == "nothing")
            {
                MessageBox.Show("request [nothing] captured!");
            }
            else
            {
                await next(context);
            }
        }
    }
}

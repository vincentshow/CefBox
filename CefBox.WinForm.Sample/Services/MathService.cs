using CefBox.Models;
using CefBox.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefBox.WinForm.Sample.Services
{
    public class MathService : IAppService
    {
        public async Task Add(AppRequest request)
        {
            var first = request.Data.Value<double>("first");
            var second = request.Data.Value<double>("second");

            await request.ExecJSCallback(() => first + second);
        }

        public async Task Divide(AppRequest request)
        {
            await request.ExecJSCallback(() =>
            {
                var first = request.Data.Value<double>("first");
                var second = request.Data.Value<double>("second");
                if (second == 0)
                {
                    throw new AppException(ExceptionCode.InvalidArgument, "divided by 0");
                }
                return first / second;
            });
        }
    }
}

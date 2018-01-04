using CefBox.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CefBox
{
    public static class AppExtensions
    {
        public static T GetService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(T));
        }

        public static AppHoster UseRouter(this AppHoster hoster, IServiceProvider provider, Func<IEnumerable<Assembly>> getTargetAssembly)
        {
            var router = provider.GetService<Router>();
            if (router == null)
            {
                throw new AppException(ExceptionCode.OperationFailed, $"cannot get instance of {nameof(Router)}");
            }

            var allAssembly = getTargetAssembly()?.ToList();
            if (allAssembly != null)
            {
                allAssembly.ForEach(target => router.RegistServices(target));
            }

            hoster.AppBuilder.UseMiddleware(() => router);
            return hoster;
        }
    }
}

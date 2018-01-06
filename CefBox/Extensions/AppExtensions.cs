using CefBox.Middlewares;
using CefBox.Models;
using CefSharp;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CefBox.Extensions
{
    public static class AppExtensions
    {
        public static T GetService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(T));
        }

        public static AppHoster UseMiddleware<T>(this AppHoster hoster, IServiceProvider provider) where T : IMiddleware
        {
            hoster.AppBuilder.UseMiddleware(() => provider.GetService<T>());
            return hoster;
        }

        public static AppHoster UseRouter(this AppHoster hoster, IServiceProvider provider, Func<IEnumerable<Assembly>> getTargetAssembly = null)
        {
            var router = provider.GetService<RouterMiddleware>();
            if (router == null)
            {
                throw new AppException(ExceptionCode.OperationFailed, $"cannot get instance of {nameof(RouterMiddleware)}");
            }

            var allAssembly = getTargetAssembly?.Invoke()?.ToList();
            if (allAssembly != null)
            {
                allAssembly.ForEach(target => router.RegistServices(target));
            }

            hoster.AppBuilder.UseMiddleware(() => router);
            return hoster;
        }

        public static string ToLogString(this AppRequest request, IEnumerable<string> filterFields = null)
        {
            var result = JsonConvert.SerializeObject(request);
            if (filterFields != null)
            {
                filterFields.ToList().ForEach(field =>
                {
                    var fieldVal = request.Data?.Value<string>(field);
                    if (!string.IsNullOrEmpty(fieldVal))
                    {
                        result = result.Replace($"\"{fieldVal}\"", "\"***\"");
                    }
                });
            }

            return result;
        }

        public static long ToTimestamp(this DateTime time)
        {
            return (long)(time.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        /// <summary>
        /// 将本地路径转化成网络可识别的绝对路径，无file:///开头
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="forLocaleUsing">是否本地解析使用，默认false：为前台显示用, true:后台使用</param>
        /// <returns></returns>
        public static string ToAppPath(this string localPath, bool forLocaleUsing = false)
        {
            var path = localPath;
            try
            {
                if (forLocaleUsing)
                {
                    path = Uri.UnescapeDataString(path);
                    if (!string.IsNullOrEmpty(GlobalConfig.Domain.LocalName))
                    {
                        path = path.Replace($"{GlobalConfig.Domain.LocalName}/", string.Empty);
                    }
                }
                else
                {
                    path = new Uri(localPath).AbsolutePath;
                    if (!string.IsNullOrEmpty(GlobalConfig.Domain.LocalName))
                    {
                        path = $"{GlobalConfig.Domain.LocalName}/{path}";
                    }
                }
            }
            catch
            {
            }

            return path;
        }

        public static bool FileExists(this string path)
        {
            return File.Exists(path);
        }

        public static bool DirExists(this string path)
        {
            return Directory.Exists(path);
        }

    }
}

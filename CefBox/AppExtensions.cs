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

namespace CefBox
{
    public static class AppExtensions
    {
        private static string globalCallbackName = "appCallback";

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

        /// <summary>
        /// 回调js方法
        /// </summary>
        /// <param name="request"></param>
        /// <param name="getLogicdata"></param>
        /// <param name="usedLater">request.Callback被调用成功后，指示js是否能清除该方法,0表示不再使用</param>
        /// <returns></returns>
        public static async Task<JavascriptResponse> ExecJSCallback(this AppRequest request, Func<object> getLogicdata, bool usedLater = false, ILogger logger = null)
        {
            var data = GetResponse(getLogicdata);
            if (!string.IsNullOrEmpty(request.Callback))
            {
                return await EvaluateScriptAsync(request, $"{globalCallbackName}('{request.Callback}', '{usedLater}', {data})", logger).ConfigureAwait(false);
            }
            return new JavascriptResponse();
        }

        /// <summary>
        /// 回调js方法
        /// </summary>
        /// <param name="request"></param>
        /// <param name="getLogicdata"></param>
        /// <param name="usedLater">request.Callback被调用成功后，指示js是否能清除该方法,0表示不再使用</param>
        /// <returns></returns>
        public static async Task<JavascriptResponse> ExecJSCallbackAsync(this AppRequest request, Func<Task<object>> getLogicDataAsync, bool usedLater = false, ILogger logger = null)
        {
            var data = await GetResponseAsync(getLogicDataAsync).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(request.Callback))
            {
                return await EvaluateScriptAsync(request, $"{globalCallbackName}('{request.Callback}', '{usedLater}', {data})", logger).ConfigureAwait(false);
            }
            return new JavascriptResponse();
        }

        private static string GetResponse(Func<object> getLogicData)
        {
            var logicResult = new AppResponse();

            try
            {
                logicResult.Data = getLogicData?.Invoke();
            }
            catch (Exception ex)
            {
                logicResult.Flag = 1;
                logicResult.Message = ex.Message;
                if (ex is AppException)
                {
                    logicResult.Code = (ex as AppException).Code;
                }
                else
                {
                    logicResult.Code = ExceptionCode.UnknownError;
                }
            }

            return JsonConvert.SerializeObject(logicResult);
        }

        private static async Task<string> GetResponseAsync(Func<Task<object>> getLogicData)
        {
            var logicResult = new AppResponse();

            try
            {
                logicResult.Data = await getLogicData?.Invoke();
            }
            catch (Exception ex)
            {
                //Log.Logger.Error(ex, "");

                logicResult.Flag = 1;
                logicResult.Message = ex.Message;
                if (ex is AppException)
                {
                    logicResult.Code = (ex as AppException).Code;
                }
                else
                {
                    logicResult.Code = ExceptionCode.UnknownError;
                }
            }

            return JsonConvert.SerializeObject(logicResult);
        }

        private static Task<JavascriptResponse> EvaluateScriptAsync(AppRequest request, string js, ILogger logger = null)
        {
            //return logger.PerformanceAsync(() =>
            //{
            if (request.Frame.Browser.IsBrowserInitialized && request.Frame.Browser.CanExecuteJavascriptInMainFrame)
            {
                return request.Frame.Browser.EvaluateScriptAsync(js);
            }
            return Task.FromResult(new JavascriptResponse());
            //}, $"exec js: {request.Callback}");
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

using CefSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CefBox
{
    public class AppRequest
    {
        public string Id { get; set; } = DateTime.Now.ToFileTimeUtc().ToString();
        /// <summary>
        /// 当前请求所在的frame
        /// </summary>
        public string FrameId { get; set; }

        /// <summary>
        /// 请求的服务名
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// 请求的方法名
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// js回调方法名
        /// </summary>
        public string Callback { get; set; }

        /// <summary>
        /// 请求方法的参数
        /// </summary>
        public JObject Data { get; set; }

        /// <summary>
        /// 当前请求所在的frame
        /// </summary>
        public IAppFrame Frame { get; set; }
    }

    public class CefRequest<TParam> : AppRequest
    {
        /// <summary>
        /// 逻辑参数
        /// </summary>
        public new TParam Data { get; set; }
    }

    public static class CefRequestExtensions
    {
        private static string globalCallbackName = "appCallback";

        public static string ToLogString(this AppRequest request)
        {
            var result = JsonConvert.SerializeObject(request);
            var password = request.Data?.Value<string>("password");
            if (!string.IsNullOrEmpty(password))
            {
                result = result.Replace($"\"{password}\"", "\"xxx\"");
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
    }
}

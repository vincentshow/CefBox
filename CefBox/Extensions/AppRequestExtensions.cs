using CefBox.Models;
using CefSharp;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace CefBox.Extensions
{
    public static class AppRequestExtensions
    {
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
                return await EvaluateScriptAsync(request, $"{GlobalConfig.AppOptions.JSCallbackName}('{request.Callback}', '{usedLater}', {data})", logger).ConfigureAwait(false);
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
                return await EvaluateScriptAsync(request, $"{GlobalConfig.AppOptions.JSCallbackName}('{request.Callback}', '{usedLater}', {data})", logger).ConfigureAwait(false);
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
                    logicResult.Code = (ex as AppException).Code.Id;
                }
                else
                {
                    logicResult.Code = ExceptionCode.UnknownError.Id;
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
                    logicResult.Code = (ex as AppException).Code.Id;
                }
                else
                {
                    logicResult.Code = ExceptionCode.UnknownError.Id;
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

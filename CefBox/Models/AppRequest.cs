using CefSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CefBox.Models
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

    public class AppRequest<TParam> : AppRequest
    {
        /// <summary>
        /// 逻辑参数
        /// </summary>
        public new TParam Data { get; set; }
    }
}

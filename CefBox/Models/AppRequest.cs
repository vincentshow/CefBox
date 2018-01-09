using CefSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CefBox.Models
{
    /// <summary>
    /// unified data used between js and cef
    /// </summary>
    public class AppRequest
    {
        public string Id { get; set; } = DateTime.Now.ToFileTimeUtc().ToString();
        /// <summary>
        /// specify the frame which request from
        /// </summary>
        public string FrameId { get; set; }

        /// <summary>
        /// request service name
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// requets action name
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// js callback function name 
        /// </summary>
        public string Callback { get; set; }

        /// <summary>
        /// request params
        /// </summary>
        public JObject Data { get; set; }

        /// <summary>
        /// frame which current request belong to
        /// </summary>
        public IAppFrame Frame { get; set; }
    }

    public class AppRequest<TParam> : AppRequest
    {
        /// <summary>
        /// logic param
        /// </summary>
        public new TParam Data { get; set; }
    }
}

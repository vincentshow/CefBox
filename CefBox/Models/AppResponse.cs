using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefBox.Models
{
    /// <summary>
    /// unified data used between cef and js
    /// </summary>
    public class AppResponse
    {
        /// <summary>
        /// the interop result, 1:success
        /// </summary>
        public int Flag { get; set; }
        /// <summary>
        /// error code
        /// </summary>
        public int Code { get; set; } = ExceptionCode.None.Id;
        /// <summary>
        /// logic message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// logic result
        /// </summary>
        public object Data { get; set; }
    }

    public class AppResponse<T> : AppResponse
    {
        /// <summary>
        /// logic result
        /// </summary>
        public new T Data { get; set; }
    }
}

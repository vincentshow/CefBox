using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefBox
{
    public class AppResponse
    {
        /// <summary>
        /// 对于前端交互请求，1表示成功
        /// 后端sdk交互时，flag对应sdk的errorcode
        /// </summary>
        public int Flag { get; set; }

        public ExceptionCode Code { get; set; } = ExceptionCode.None;

        public string Message { get; set; }

        public object Data { get; set; }
    }

    public class CefResponse<T> : AppResponse
    {
        public new T Data { get; set; }
    }
}

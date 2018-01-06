using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefBox.Models
{
    public class AppResponse
    {
        /// <summary>
        /// 对于前端交互请求，1表示成功
        /// 后端sdk交互时，flag对应sdk的errorcode
        /// </summary>
        public int Flag { get; set; }

        public int Code { get; set; } = ExceptionCode.None.Id;

        public string Message { get; set; }

        public object Data { get; set; }
    }

    public class AppResponse<T> : AppResponse
    {
        public new T Data { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefBox
{
    public class AppException : Exception
    {
        public ExceptionCode Code { get; set; }

        public AppException(ExceptionCode code = ExceptionCode.None, string info = null) :
            base(info)
        {
            this.Code = code;
        }
    }
}

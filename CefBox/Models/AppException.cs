using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefBox.Models
{
    public class AppException : Exception
    {
        public ExceptionCode Code { get; set; }

        public AppException(ExceptionCode code = null, string info = null) :
            base(info)
        {
            this.Code = code ?? ExceptionCode.None;
        }
    }
}

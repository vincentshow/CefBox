using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefBox.Models
{
    public class ExceptionCode : AppEnumeration
    {
        private static int _sysSeed = 0;
        public static ExceptionCode None = new ExceptionCode(_sysSeed++, nameof(None));
        public static ExceptionCode NotImplemented = new ExceptionCode(_sysSeed++, nameof(NotImplemented));
        public static ExceptionCode OperationFailed = new ExceptionCode(_sysSeed++, nameof(OperationFailed));
        public static ExceptionCode InvalidArgument = new ExceptionCode(_sysSeed++, nameof(InvalidArgument));
        public static ExceptionCode InvalidOperation = new ExceptionCode(_sysSeed++, nameof(InvalidOperation));

        public static ExceptionCode UnknownError = new ExceptionCode(99999, nameof(UnknownError));

        protected ExceptionCode() { }

        public ExceptionCode(int id, string name)
            : base(id, name)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CefBox.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
    public class AppServiceAttribute : System.Attribute
    {
        public string Name { get; private set; }

        public AppServiceAttribute(string name)
        {
            this.Name = name;
        }
    }
}

using System;
using System.Runtime.CompilerServices;

namespace CefBox.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AppActionAttribute : System.Attribute
    {
        public string Name { get; private set; }

        public bool ForbiddenWhenUpdating { get; private set; }

        public bool IMMustOnline { get; set; }

        public AppActionAttribute(string name = null, bool forbiddenWhenUpdating = false,bool imMustOnline = false, [CallerMemberName] string caller = null)
        {
            this.Name = name ?? caller;
            this.ForbiddenWhenUpdating = forbiddenWhenUpdating;
            this.IMMustOnline = imMustOnline;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefBox.Models
{
    public class Domain
    {
        public string Scheme { get; set; }

        public string EmbeddedName { get; set; }

        public string LocalName { get; set; }

        public string ResAssemblyName { get; set; }

        public string ResNamespace { get; set; }
    }
}

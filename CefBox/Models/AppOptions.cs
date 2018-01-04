using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefBox.Models
{
    public class AppOptions
    {
        public string Name { get; set; } = "CefBox";

        public double MiniVersion { get; set; }

        public string Scheme { get; set; } = "app";

        public string ResAssemblyName { get; set; }

        public string ResNamespace { get; set; }

        public readonly string HomePath = AppDomain.CurrentDomain.BaseDirectory;

        private string _dataPath;
        public string DataPath
        {
            get
            {
                if (string.IsNullOrEmpty(_dataPath))
                {
                    _dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Name);
                }
                return _dataPath;
            }
            set
            {
                _dataPath = value;
            }
        }
    }
}

using CefBox.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CefBox
{
    public class GlobalConfig
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        #region global config

        public static string AppName { get; set; }

        private static double _miniVersion = 1.00;

        private static double _appVersion = 0;
        public static double AppVersion
        {
            get
            {
                if (_appVersion == 0)
                {
                    _appVersion = _miniVersion;
                    var confVersion = GetConfig<string>("global", "version");

                    if (!string.IsNullOrEmpty(confVersion))
                    {
                        _appVersion = Math.Max(double.Parse(confVersion), _appVersion);
                    }
                    WriteConfig("global", "version", _appVersion);
                }
                return _appVersion;
            }
        }

        #endregion

        #region path

        public static string HomePath { get; private set; } = AppDomain.CurrentDomain.BaseDirectory;

        private static Domain _domain;
        public static Domain Domain
        {
            get
            {
                if (_domain == null)
                {
                    _domain = new Domain
                    {
                        Scheme = "app",
                        ResAssemblyName = $"{AppName}Res.dll",
                        ResNamespace = $"{AppName}Res"
                    };

                    var basePath = GetConfig<string>("debug", "contentpath");
                    if (string.IsNullOrEmpty(basePath))
                    {
                        _domain.EmbeddedName = $"{_domain.Scheme}://embedded";
                        _domain.LocalName = $"{_domain.Scheme}://local";
                    }
                    else
                    {
                        _domain.EmbeddedName = Path.GetFullPath(basePath);
                        _domain.LocalName = string.Empty;
                    }
                }
                return _domain;
            }
        }

        public static string DataPath { get; private set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppName);

        private static int _level;
        public static LogLevel LogLevel
        {
            get
            {
                _level = GetConfig<int>("global", "logLevel");
                if (_level == 0)
                {
                    _level = (int)LogLevel.Warn;
                    WriteConfig("global", "logLevel", _level);
                }
                return (LogLevel)_level;
            }
        }

        #endregion

        private static string _clientConfigPath = Path.Combine(HomePath, "config", "client.ini");

        public static string GetConfig(string sessionName, string paraName, string path = null)
        {
            path = path ?? _clientConfigPath;

            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(sessionName, paraName, "", temp, 255, path);
            return temp.ToString();
        }

        public static T GetConfig<T>(string sessionName, string paraName, string path = null)
        {
            var result = default(T);
            var strResult = GetConfig(sessionName, paraName, path);
            if (!string.IsNullOrEmpty(strResult))
            {
                result = (T)Convert.ChangeType(strResult, typeof(T));
            }
            return result;
        }

        public static bool WriteConfig(string sessionName, string paraName, string value, string path = null)
        {
            path = path ?? _clientConfigPath;
            if (!path.FileExists())
            {
                if (!Path.GetDirectoryName(path).DirExists())
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                File.Create(path);
            }

            return WritePrivateProfileString(sessionName, paraName, value, _clientConfigPath);
        }

        public static bool WriteConfig(string sessionName, string paraName, object value, string path = null)
        {
            return WriteConfig(sessionName, paraName, value.ToString(), path);
        }
    }

    public enum LogLevel
    {
        None = 0,
        Trace = 1,
        Debug = 2,
        Info = 3,
        Warn = 4,
        Error = 5
    }
}

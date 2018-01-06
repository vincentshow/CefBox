using CefBox.Models;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CefBox
{
    public class GlobalConfig
    {
        public static AppOptions AppOptions { get; set; }

        #region global config

        private static double _appVersion = 0;
        public static double AppVersion
        {
            get
            {
                if (_appVersion == 0)
                {
                    _appVersion = AppOptions.MiniVersion;
                    var confVersion = AppConfiguration.GetConfig<string>("global.version");

                    if (!string.IsNullOrEmpty(confVersion))
                    {
                        _appVersion = Math.Max(double.Parse(confVersion), _appVersion);
                    }
                    AppConfiguration.WriteConfig("global.version", _appVersion);
                }
                return _appVersion;
            }
        }

        #endregion

        #region path

        private static Domain _domain;
        public static Domain Domain
        {
            get
            {
                if (_domain == null)
                {
                    _domain = new Domain
                    {
                        Scheme = AppOptions.Scheme,
                        ResAssemblyName = AppOptions.ResAssemblyName,
                        ResNamespace = AppOptions.ResNamespace
                    };

                    var basePath = AppConfiguration.GetConfig<string>("debug.contentPath");
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

        private static int _level;
        public static LogLevel LogLevel
        {
            get
            {
                _level = AppConfiguration.GetConfig<int>("global.logLevel");
                if (_level == 0)
                {
                    _level = (int)LogLevel.Warn;
                    AppConfiguration.WriteConfig("global.logLevel", _level);
                }
                return (LogLevel)_level;
            }
        }

        #endregion
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

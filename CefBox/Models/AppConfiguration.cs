using CefBox.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CefBox.Models
{
    /// <summary>
    /// configuration manager, using ini format file 
    /// </summary>
    public class AppConfiguration
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        private static string _configFilePath = null;
        /// <summary>
        /// the config file path, default settings.ini
        /// </summary>
        public static string ConfigFilePath
        {
            get
            {
                if (_configFilePath == null)
                {
                    _configFilePath = Path.GetFullPath("settings.ini");
                }
                return _configFilePath;
            }
            set
            {
                _configFilePath = value;
            }
        }

        #region get conf
        /// <summary>
        /// get config value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionKey">format {section}.{key}</param>
        /// <param name="path">config file path, use ConfigFilePath when null</param>
        /// <returns></returns>
        public static T GetConfig<T>(string sectionKey, string path = null)
        {
            var param = ParseSectionKey(sectionKey);
            return GetConfig<T>(param[0], param[1], path);
        }

        public static T GetConfig<T>(string section, string key, string path = null)
        {
            var result = default(T);
            var strResult = GetConfig(section, key, path);
            if (!string.IsNullOrEmpty(strResult))
            {
                result = (T)Convert.ChangeType(strResult, typeof(T));
            }
            return result;
        }

        public static string GetConfig(string section, string key, string path = null)
        {
            path = path ?? ConfigFilePath;

            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", temp, 255, path);
            return temp.ToString();
        }

        #endregion

        #region write conf

        public static bool WriteConfig(string section, string key, object value, string path = null)
        {
            return WriteConfig(section, key, value.ToString(), path);
        }

        public static bool WriteConfig(string sectionKey, object value, string path = null)
        {
            var param = ParseSectionKey(sectionKey);
            return WriteConfig(param[0], param[1], value.ToString(), path);
        }

        public static bool WriteConfig(string section, string key, string value, string path = null)
        {
            path = path ?? ConfigFilePath;
            if (!path.FileExists())
            {
                if (!Path.GetDirectoryName(path).DirExists())
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                File.Create(path);
            }

            return WritePrivateProfileString(section, key, value, path);
        }

        #endregion

        private static string[] ParseSectionKey(string sectionKey)
        {
            if (string.IsNullOrEmpty(sectionKey) || sectionKey.Split('.').Length < 2)
            {
                throw new ArgumentException($"invalid {nameof(sectionKey)}, correct format is {{session}}.{{key}}");
            }

            return sectionKey.Split('.');
        }
    }
}

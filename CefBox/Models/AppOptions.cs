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
        /// <summary>
        /// app name, using in code, default CefBox
        /// </summary>
        public string Name { get; set; } = "CefBox";
        /// <summary>
        /// app title, using to show msg to users
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// render app fullname, default CefSharp.BrowserSubprocess.exe. set this value when you custom yourself render
        /// </summary>
        public string RenderName { get; set; } = "CefSharp.BrowserSubprocess.exe";
        /// <summary>
        /// miniversion of this app, useful when updating
        /// </summary>
        public double MiniVersion { get; set; }
        /// <summary>
        /// scheme that will be intercepted by cef, default app. That is when you request resource like "app://xxx" will be processed in cef.
        /// </summary>
        public string Scheme { get; set; } = "app";
        /// <summary>
        /// if embed front resource(.html, .css, .js etc.) in dll, set this value to the dll fullname that contains static resource.
        /// Disable this by specify the contentPath in settings.ini
        /// </summary>
        public string ResAssemblyName { get; set; }
        /// <summary>
        /// base namespace of the embedded resource
        /// </summary>
        public string ResNamespace { get; set; }
        /// <summary>
        /// name of the object injected into js, default app. That is you must call app.fetch(...) to get data from cef
        /// </summary>
        public string InjectObjName { get; set; } = "app";
        /// <summary>
        /// when request completed or updating something, cef will call this js function to tell front.
        /// </summary>
        public string JSCallbackName { get; set; } = "appCallback";
        /// <summary>
        /// id that identify the main frame
        /// </summary>
        public string MainFrameId { get; set; } = "mainFrame";
        /// <summary>
        /// app home path
        /// </summary>
        public readonly string HomePath = AppDomain.CurrentDomain.BaseDirectory;

        private string _dataPath;
        /// <summary>
        /// app temp path, default $CurrentUser$/AppData/local/{Name}
        /// </summary>
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

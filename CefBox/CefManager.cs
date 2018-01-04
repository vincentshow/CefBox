using CefSharp;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using CefBox.Models;
using CefBox.CefHandlers;

namespace CefBox
{
    public class CefManager
    {
        private static bool isCleaning;
        private static IDictionary<int, Tuple<long, int>> _reloadHistory;

        public static string UserDataPath = Path.Combine(GlobalConfig.DataPath, "webData");

        public static void Init(Action<CefSettings> customSetting = null)
        {
            if (Cef.IsInitialized)
            {
                return;
            }

            //For Windows 7 and above, best to include relevant app.manifest entries as well
            Cef.EnableHighDPISupport();
            CefSharpSettings.WcfEnabled = false;

            var settings = new CefSettings
            {
                LogSeverity = LogSeverity.Error,
                RemoteDebuggingPort = 8900,
                FocusedNodeChangedEnabled = true,
                CachePath = Path.Combine(GlobalConfig.DataPath, "webCache"),
                UserDataPath = UserDataPath,
                LogFile = Path.Combine(UserDataPath, $"{DateTime.Now.ToString("yyyyMMdd")}.log"),
            };

            //settings.CefCommandLineArgs.Add("disable-web-security", "1");
            //settings.CefCommandLineArgs.Add("allow-file-access-from-files", "1");
            //settings.CefCommandLineArgs.Add("no-proxy-server", "1");
            //settings.CefCommandLineArgs.Remove("process-per-tab");

            //settings.CefCommandLineArgs.Add("disable-accelerated-compositing", "1");
            settings.CefCommandLineArgs.Add("disable-application-cache", "1");
            settings.CefCommandLineArgs.Add("disable-restore-background-contents", "1");
            settings.CefCommandLineArgs.Add("no-proxy-server", "1"); //Don't use a proxy server, always make direct connections. Overrides any other proxy server flags that are passed.
            settings.CefCommandLineArgs.Add("disable-plugins-discovery", "1"); //Disable discovering third-party plugins. Effectively loading only ones shipped with the browser plus third-party ones as specified by --extra-plugin-dir and --load-plugin switches

            settings.CefCommandLineArgs.Add("disable-gpu-vsync", "1");
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            //settings.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
            //settings.CefCommandLineArgs.Add("enable-begin-frame-scheduling", "1");
            //settings.CefCommandLineArgs.Add("renderer-process-limit", "2");

            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = GlobalConfig.Domain.Scheme,
                SchemeHandlerFactory = new SchemeHandlerFactory()
            });

            customSetting?.Invoke(settings);

            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
        }

        public static void LoadBrowser(IWebBrowser browser, CefOptions cefOptions)
        {
            if (browser != null)
            {
                if (cefOptions.LifeSpanHandler != null)
                {
                    browser.LifeSpanHandler = cefOptions.LifeSpanHandler;
                }

                if (cefOptions.MenuHandler != null)
                {
                    browser.MenuHandler = cefOptions.MenuHandler;
                }

                if (cefOptions.DragHandler != null)
                {
                    browser.DragHandler = cefOptions.DragHandler;
                }

                if (cefOptions.JsDialogHandler != null)
                {
                    browser.JsDialogHandler = cefOptions.JsDialogHandler;
                }

                if (cefOptions.KeyboardHandler != null)
                {
                    browser.KeyboardHandler = cefOptions.KeyboardHandler;
                }

                browser.RequestHandler = new DefaultRequestHandler();
                //browser.ConsoleMessage += Browser_ConsoleMessage;
                if (!string.IsNullOrEmpty(cefOptions.InjectObjName))
                {
                    var injectObj = cefOptions.GetInjectObj();
                    if (injectObj == null)
                    {
                        //GTLogger.Instance.Log("inject object is null");
                    }
                    else
                    {
                        browser.RegisterJsObject(cefOptions.InjectObjName, injectObj);
                    }
                }
            }
        }

        public static void Shutdown()
        {
            Cef.Shutdown();
        }

        /// <summary>
        /// 对于crash的render，同一个content一分钟之内只能reload3次
        /// </summary>
        /// <param name="browserControl"></param>
        /// <returns></returns>
        public static bool ReloadBrowser(IWebBrowser browserControl)
        {
            ClearReloadHistory();

            var addrKey = browserControl.Address.GetHashCode();
            Tuple<long, int> history = null;
            var counter = 1;
            var current = DateTime.Now;
            var timestamp = current.ToTimestamp();
            var canReload = true;
            if (_reloadHistory.TryGetValue(addrKey, out history))
            {
                canReload = false;
                if (history.Item1 < current.AddMinutes(-1).ToTimestamp())
                {
                    canReload = true;
                }
                else if (history.Item2 < 3)
                {
                    timestamp = history.Item1;
                    counter = history.Item2 + 1;
                    canReload = true;
                }
            }

            if (canReload)
            {
                _reloadHistory.Remove(addrKey);
                history = Tuple.Create(timestamp, counter);
                _reloadHistory.Add(addrKey, history);
                browserControl.Reload();
            }
            return canReload;
        }

        private static void ClearReloadHistory()
        {
            if (isCleaning)
            {
                return;
            }

            try
            {
                isCleaning = true;
                if (_reloadHistory != null)
                {
                    var miniTimestamp = DateTime.Now.AddMinutes(-1).ToTimestamp();
                    var keys = _reloadHistory.Keys.ToList();
                    Tuple<long, int> tuple = null;
                    foreach (var key in keys)
                    {
                        if (_reloadHistory.TryGetValue(key, out tuple) && tuple.Item1 < miniTimestamp)
                        {
                            _reloadHistory.Remove(key);
                        }
                    }
                }
                else
                {
                    _reloadHistory = new ConcurrentDictionary<int, Tuple<long, int>>();
                }
            }
            finally
            {
                isCleaning = false;
            }
        }
    }
}

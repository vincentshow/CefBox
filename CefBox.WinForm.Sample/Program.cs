using CefBox.Extensions;
using CefBox.Middlewares;
using CefBox.Models;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CefBox.WinForm.Sample
{
    static class Program
    {
        public static Container DIContainer;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GlobalConfig.AppOptions = new AppOptions
            {
                ResAssemblyName = "CefBox.WinForm.Sample.exe",
                ResNamespace = "CefBox.WinForm.Sample.Res"
            };
            AppConfiguration.ConfigFilePath = Path.Combine(GlobalConfig.AppOptions.HomePath, "settings.ini");
            var form = new SampleForm();

            DIContainer = new Container();
            DIContainer.Options.DefaultLifestyle = Lifestyle.Singleton;

            DIContainer.Register<IAppFrame>(() => form);
            DIContainer.Register<IServiceProvider>(() => DIContainer);
            DIContainer.Register<ILoggerFactory>(() => new LoggerFactory());

            RegisterService();
            RegisterMiddleware();

            AppHoster.Instance.UseMiddleware<LoggerMiddleware>(DIContainer);
            AppHoster.Instance.UseMiddleware<FrameMiddleware>(DIContainer);

            AppHoster.Instance.UseRouter(DIContainer, () => new List<Assembly> { Assembly.GetExecutingAssembly() });

            CefManager.Init();

            Application.Run(form);
        }

        private static void RegisterService()
        {
            var targetAssembly = Assembly.GetAssembly(typeof(IAppService));
            var registrations = targetAssembly.GetExportedTypes()
                                              .Where(type => type.BaseType != null && type.BaseType == typeof(IAppService));
            foreach (var reg in registrations)
            {
                DIContainer.Register(reg);
            }
        }

        private static void RegisterMiddleware()
        {
            var targetAssembly = Assembly.GetAssembly(typeof(IMiddleware));
            var registrations = targetAssembly.GetExportedTypes()
                                              .Where(type => type.GetInterfaces() != null && type.GetInterfaces().Any(item => item == typeof(IMiddleware)));
            foreach (var reg in registrations)
            {
                DIContainer.Register(reg);
            }
        }
    }
}

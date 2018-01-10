using CefBox.Extensions;
using CefBox.Middlewares;
using CefBox.Models;
using CefBox.WinForm.Sample.Middlewares;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace CefBox.WinForm.Sample
{
    static class Program
    {
        public static IServiceProvider ServiceProvider;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GlobalSetting();
            ServiceProvider = DISetting();
            HosterSetting();

            Application.Run(ServiceProvider.GetService<IAppFrame>() as Form);
        }

        private static void GlobalSetting()
        {
            GlobalConfig.AppOptions = new AppOptions
            {
                Name = "CefSample",
                Title = "AppTitle",
                ResAssemblyName = "CefBox.WinForm.Sample.exe",
                ResNamespace = "CefBox.WinForm.Sample.Res"
            };

            //set config file path if neccesary
            AppConfiguration.ConfigFilePath = Path.Combine(GlobalConfig.AppOptions.HomePath, "settings.ini");

            CefManager.Init(settngs =>
            {
                //add other settings if neccesary
            });
        }

        private static IServiceProvider DISetting()
        {
            var form = new SampleForm();

            var container = new Container();
            container.Options.DefaultLifestyle = Lifestyle.Singleton;

            container.Register<IAppFrame>(() => form);
            container.Register<IServiceProvider>(() => container);
            container.Register<ILoggerFactory>(() => new LoggerFactory());

            RegisterService(container);
            RegisterMiddleware(container);

            return container;
        }

        private static void HosterSetting()
        {
            //register default middlewares
            AppHoster.Instance.UseMiddleware<LoggerMiddleware>(ServiceProvider);
            AppHoster.Instance.UseMiddleware<FrameMiddleware>(ServiceProvider);

            //register middlewares youself here if neccesary
            AppHoster.Instance.UseMiddleware<SampleMiddleware>(ServiceProvider);

            //this will register all available router and map to relevant service and action
            AppHoster.Instance.UseRouter(ServiceProvider, () => new List<Assembly> { Assembly.GetExecutingAssembly() });
        }

        private static void RegisterService(Container container)
        {
            var targetAssembly = Assembly.GetAssembly(typeof(IAppService));
            var registrations = targetAssembly.GetExportedTypes()
                                              .Where(type => type.BaseType != null && type.BaseType == typeof(IAppService));
            foreach (var reg in registrations)
            {
                container.Register(reg);
            }
        }

        private static void RegisterMiddleware(Container container)
        {
            //inject default middlewares
            var targetAssemblys = new List<Assembly> { Assembly.GetAssembly(typeof(IMiddleware)) };

            //add others if neccesary
            targetAssemblys.Add(Assembly.GetExecutingAssembly());

            //get target types
            var registrations = new List<Type>();
            targetAssemblys.ForEach(assembly =>
            {
                var types = assembly.GetExportedTypes()
                                               .Where(type => type.GetInterfaces() != null
                                                              && type.GetInterfaces().Any(item => item == typeof(IMiddleware)));
                if (types != null && types.Count() > 0)
                {
                    registrations.AddRange(types);
                }
            });

            foreach (var reg in registrations)
            {
                container.Register(reg);
            }
        }
    }
}


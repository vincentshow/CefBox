using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using CefBox.Attributes;

namespace CefBox.Middlewares
{
    public class Router : IMiddleware
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;

        private readonly IDictionary<string, IEnumerable<Type>> serviceMapper;
        /// <summary>
        /// action集合，key是route path，value分别存储action所在的类名，action对应的方法名，也可以扩展存储action的其他属性定义（如权限，是否在线验证等）
        /// </summary>
        private readonly IDictionary<string, Tuple<string, string>> actionMapper;

        public Router(ILoggerFactory factory, IServiceProvider provider)
        {
            _logger = factory.CreateLogger(this.GetType());
            _provider = provider;

            serviceMapper = new ConcurrentDictionary<string, IEnumerable<Type>>();
            actionMapper = new ConcurrentDictionary<string, Tuple<string, string>>();

            RegistServices();
        }

        public async Task InvokeAsync(AppRequest context, RequestDelegate next)
        {
            IEnumerable<Type> services;
            var serviceName = $"{context.Service.ToLower()}";
            if (!serviceMapper.TryGetValue(serviceName, out services))
            {
                return;
            }

            foreach (var service in services)
            {
                var instance = _provider.GetService(service);
                if (instance == null)
                {
                    throw new AppException(ExceptionCode.NotImplemented, context.Service);
                }

                Tuple<string, string> action;
                if (!actionMapper.TryGetValue($"{serviceName}/{context.Action.ToLower()}", out action))
                {
                    throw new AppException(ExceptionCode.NotImplemented, $"{context.Service}.{context.Action}");
                }

                if (service.FullName != action.Item1)
                {
                    continue;
                }

                var result = service.InvokeMember(action.Item2, BindingFlags.InvokeMethod, Type.DefaultBinder, instance, new object[] { context });
                if (result != null)
                {
                    await (result as Task);
                    break;
                }
            }
            await next(context);
        }

        public void RegistServices(Assembly targetAssembly = null)
        {
            if (targetAssembly == null)
            {
                targetAssembly = Assembly.GetAssembly(typeof(IAppService));
            }

            var serviceTypes = targetAssembly.GetExportedTypes()
                                             .Where(type => type.BaseType != null && type.BaseType == typeof(IAppService));
            foreach (var service in serviceTypes)
            {
                var serviceName = service.Name;
                if (serviceName.EndsWith("service", StringComparison.OrdinalIgnoreCase))
                {
                    serviceName = serviceName.Substring(0, serviceName.Length - 7);
                }

                var serviceAttr = service.GetCustomAttribute<AppServiceAttribute>();
                if (serviceAttr != null && !string.IsNullOrEmpty(serviceAttr.Name))
                {
                    serviceName = serviceAttr.Name;
                }
                serviceName = serviceName.ToLower();

                var services = new List<Type> { service };
                if (serviceMapper.ContainsKey(serviceName))
                {
                    services.AddRange(serviceMapper[serviceName]);
                    serviceMapper.Remove(serviceName);
                }
                serviceMapper.Add(serviceName, services);

                var methods = service.GetMethods();
                if (methods != null || methods.Length > 0)
                {
                    RegistActions(serviceName, service.FullName, methods);
                }
            }
        }

        private void RegistActions(string serviceName, string typeName, MethodInfo[] methods)
        {
            foreach (var method in methods)
            {
                if (!method.IsPublic || method.ReturnType == null
                    || !method.ReturnType.IsAssignableFrom(typeof(Task)))
                {
                    continue;
                }

                var param = method.GetParameters();
                if (param == null || param.Length != 1)
                {
                    continue;
                }

                if (param[0].ParameterType.IsAssignableFrom(typeof(AppRequest)))
                {
                    var actionName = method.Name;
                    var actionForbidden = false;
                    var actionAttr = method.GetCustomAttribute<AppActionAttribute>();
                    if (actionAttr != null && !string.IsNullOrEmpty(actionAttr.Name))
                    {
                        actionName = actionAttr.Name;
                        actionForbidden = actionAttr.ForbiddenWhenUpdating;
                    }
                    actionMapper.Add($"{serviceName}/{actionName.ToLower()}", Tuple.Create(typeName, method.Name));
                }
            }
        }

    }
}

using CefBox.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace CefBox
{
    public class AppHoster
    {
        private static AppHoster _instance = new AppHoster();
        public static AppHoster Instance
        {
            get
            {
                return _instance;
            }
        }

        public AppBuilder AppBuilder { get; private set; }

        private AppHoster()
        {
            AppBuilder = new AppBuilder();
        }

        public async void Fetch(string route, string param)
        {
            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentNullException(nameof(route));
            }

            var request = JsonConvert.DeserializeObject<AppRequest>(param);
            if (request == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            route = route.Trim().ToLower();
            var service = route;
            var action = string.Empty;
            var actionSplitter = route.IndexOf("/");
            if (actionSplitter > 0)
            {
                service = route.Substring(0, actionSplitter);
                action = route.Substring(actionSplitter + 1);
            }

            request.Service = service.Trim();
            request.Action = action.Trim();

            await Task.Run(async () =>
            {
                await AppBuilder.Build().Invoke(request);
            }).ConfigureAwait(false);
        }
    }
}

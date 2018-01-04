using CefSharp;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace CefBox.CefHandlers
{
    public class EmbeddedResHandler : ResourceHandler
    {
        private static Assembly CurrentAssembly;
        private static string ResNamespace;

        static EmbeddedResHandler()
        {
            CurrentAssembly = Assembly.LoadFrom(Path.Combine(GlobalConfig.HomePath, GlobalConfig.Domain.ResAssemblyName));
            ResNamespace = GlobalConfig.Domain.ResNamespace;
        }

        public override bool ProcessRequestAsync(IRequest request, ICallback callback)
        {
            var domain = GlobalConfig.Domain.EmbeddedName;
            if (!request.Url.StartsWith(domain, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            Task.Run(() =>
            {
                // In this task you can perform your time consuming operations, e.g query a database
                // NOTE: We suggest that you wrap callbacks in a using statemnt so that they're disposed
                // even if there is an exception as they wrap an unmanaged response which will cause memory
                // leaks if not freed
                using (callback)
                {
                    var url = request.Url.Replace(domain, string.Empty);
                    var requestRes = this.GetEmbeddedResName(url.ToAppPath(true));
                    var stream = CurrentAssembly.GetManifestResourceStream(requestRes);
                    if (stream == null)
                    {
                        callback.Cancel();
                    }
                    else
                    {
                        //Reset the stream position to 0 so the stream can be copied into the underlying unmanaged buffer
                        stream.Position = 0;
                        //Populate the response values - No longer need to implement GetResponseHeaders (unless you need to perform a redirect)
                        ResponseLength = stream.Length;
                        MimeType = GetMimeType(Path.GetExtension(requestRes));
                        StatusCode = (int)HttpStatusCode.OK;
                        Stream?.Dispose();
                        Stream = stream;

                        callback.Continue();
                    }
                }
            }).ConfigureAwait(false);

            return true;
        }

        private string GetEmbeddedResName(string requestResName)
        {
            var paramIndex = requestResName.IndexOf("?");
            if (paramIndex > -1)
            {
                requestResName = requestResName.Substring(0, paramIndex);
            }

            paramIndex = requestResName.IndexOf("#");
            if (paramIndex > -1)
            {
                requestResName = requestResName.Substring(0, paramIndex);
            }

            if (!requestResName.StartsWith("/"))
            {
                requestResName = $"/{requestResName}";
            }

            var resName = Path.GetFileName(requestResName);
            paramIndex = requestResName.LastIndexOf(resName);

            var resPath = requestResName.Remove(paramIndex);
            return ResNamespace + resPath.Replace("-", "_").Replace(".", "._").Replace("/", ".") + resName;
        }

        public override void Dispose()
        {
            Stream?.Dispose();
        }
    }
}

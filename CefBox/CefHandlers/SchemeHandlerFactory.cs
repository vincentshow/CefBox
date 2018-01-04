using CefSharp;
using System;
using System.IO;

namespace CefBox.CefHandlers
{
    public class SchemeHandlerFactory : ISchemeHandlerFactory
    {
        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            if (string.Compare(schemeName, GlobalConfig.Domain.Scheme, true) != 0)
            {
                return null;
            }

            try
            {
                var filePath = new Uri(request.Url).AbsolutePath.Substring(1);
                if (Path.IsPathRooted(filePath))
                {
                    filePath = filePath.ToAppPath(true);
                    if (filePath.FileExists())
                    {
                        return ResourceHandler.FromFilePath(filePath, ResourceHandler.GetMimeType(Path.GetExtension(filePath)));
                    }
                    return null;
                }
                else
                {
                    return new EmbeddedResHandler();
                }
            }
            catch (Exception ex)
            {
                //GTLogger.Instance.Log("error when create IResourceHandler", ex);
                return null;
            }
        }
    }
}

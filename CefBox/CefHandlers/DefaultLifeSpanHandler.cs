using CefSharp;
using System;
using System.Diagnostics;

namespace Snail.GT.Service.CEF
{
    public class DefaultLifeSpanHandler : ILifeSpanHandler
    {
        bool ILifeSpanHandler.OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = null;
            try
            {
                Process.Start(targetUrl);
            }
            catch (Exception ex)
            {
                //如果找不到默认程序，强制使用IE打开
                //JJLogger.Instance.Log($"error when popup {targetUrl}", ex);
                try
                {
                    Process.Start("iexplore.exe", targetUrl);
                }
                catch (Exception ix)
                {
                    //JJLogger.Instance.Log($"error when popup {targetUrl} by ie", ix);
                }
            }
            return true;
        }

        void ILifeSpanHandler.OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {

        }

        bool ILifeSpanHandler.DoClose(IWebBrowser browserControl, IBrowser browser)
        {
            //We need to allow popups to close
            //If the browser has been disposed then we'll just let the default behaviour take place
            if (browser.IsDisposed || browser.IsPopup)
            {
                return false;
            }

            //The default CEF behaviour (return false) will send a OS close notification (e.g. WM_CLOSE).
            //See the doc for this method for full details.
            //return true here to handle closing yourself (no WM_CLOSE will be sent).
            return true;
        }

        public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
        {

        }
    }
}

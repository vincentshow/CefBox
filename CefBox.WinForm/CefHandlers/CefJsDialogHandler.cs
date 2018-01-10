using CefSharp;
using System.Windows.Forms;

namespace CefBox.WinForm.CefHandlers
{
    public class CefJsDialogHandler : IJsDialogHandler
    {
        private readonly IAppFrame _targetForm;
        public CefJsDialogHandler(IAppFrame target)
        {
            this._targetForm = target;
        }

        public void OnDialogClosed(IWebBrowser browserControl, IBrowser browser)
        {
        }

        public bool OnJSBeforeUnload(IWebBrowser browserControl, IBrowser browser, string message, bool isReload, IJsDialogCallback callback)
        {
            return true;
        }

        public bool OnJSDialog(IWebBrowser browserControl, IBrowser browser, string originUrl, CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage)
        {
            var result = false;
            var form = this._targetForm as Form;
            try
            {
                if (form != null)
                {
                    form.InvokeActionSafely(() =>
                    {
                        form.Focus();
                        form.TopMost = true;
                    });
                }

                var title = GlobalConfig.AppOptions.Title ?? GlobalConfig.AppOptions.Name ?? "CefBox";
                if (dialogType == CefJsDialogType.Alert)
                {
                    //todo title is configed
                    MessageBox.Show(messageText, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    suppressMessage = true;
                    result = false;
                }
                else if (dialogType == CefJsDialogType.Confirm)
                {
                    var flag = MessageBox.Show(messageText, title, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    callback?.Continue(flag == DialogResult.Yes);
                    result = true;
                }
                return result;
            }
            finally
            {
                if (form != null)
                {
                    form.InvokeActionSafely(() => form.TopMost = false);
                }
            }
        }

        public void OnResetDialogState(IWebBrowser browserControl, IBrowser browser)
        {
        }
    }
}

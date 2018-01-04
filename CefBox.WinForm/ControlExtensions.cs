using CefBox.CefHandlers;
using CefBox.Models;
using CefBox.WinForm.CefHandlers;
using System;
using System.Windows.Forms;

namespace CefBox.WinForm
{
    public static class ControlExtensions
    {
        public static void InvokeActionSafely(this Control control, Action handler)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(handler);
            }
            else
            {
                handler();
            }
        }

        public static CefOptions GetCefOptions(this IAppFrame frame, FrameOptions options, FrameOptions oldOptions = null, object injectObj = null)
        {
            var cefOptions = new CefOptions
            {
                ContentPath = options.ContentPath,
                InjectObjName = injectObj == null ? null : "app",
                GetInjectObj = () => injectObj
            };

            if (oldOptions == null)
            {
                cefOptions.JsDialogHandler = new CefJsDialogHandler(frame);
            }

            //todo cef加载完成后，handler不可改
            //因draggable是可改选项，所以要加上这个判断，以减少创建CefDragHandler实例数，满足此条件代表之前的窗口没有设置过
            if ((oldOptions == null || !oldOptions.Draggable) && options.Draggable)
            {
                cefOptions.DragHandler = new CefDragHandler(frame);
            }

            if (options.IsMain == true)
            {
                cefOptions.KeyboardHandler = new CefKeyboardHandler();
            }

            return cefOptions;
        }
    }
}

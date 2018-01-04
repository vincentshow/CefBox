using CefBox.Handlers;
using CefSharp;
using System;
using System.IO;

namespace CefBox.Models
{
    public class CefOptions
    {
        public string ContentPath { get; set; }

        public bool IsRelativeContent { get; set; } = true;

        private string _basePath;
        public string BasePath
        {
            get
            {
                if (string.IsNullOrEmpty(_basePath))
                {
                    _basePath = GlobalConfig.Domain.EmbeddedName;
                }
                return _basePath;
            }
        }

        public string InjectObjName { get; set; }

        public Func<object> GetInjectObj { get; set; }

        public IKeyboardHandler KeyboardHandler { get; set; }

        public IJsDialogHandler JsDialogHandler { get; set; } = new DefaultJsDialogHandler();

        public ILifeSpanHandler LifeSpanHandler { get; set; } = new DefaultLifeSpanHandler();

        public IDragHandler DragHandler { get; set; } = new DefaultDragHandler();

        public IContextMenuHandler MenuHandler { get; set; } = new ContextMenuHandler();
    }
}

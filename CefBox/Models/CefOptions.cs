using CefBox.Handlers;
using CefSharp;
using System;
using System.IO;

namespace CefBox.Models
{
    /// <summary>
    /// cef relevant settings
    /// </summary>
    public class CefOptions
    {
        /// <summary>
        /// the content path that will be loaded in cef
        /// </summary>
        public string ContentPath { get; set; }
        /// <summary>
        /// whether the path is relative
        /// </summary>
        public bool IsRelativeContent { get; set; } = true;

        private string _basePath;
        /// <summary>
        /// used to combine full content path
        /// </summary>
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
        /// <summary>
        /// the injected object name
        /// </summary>
        public string InjectObjName { get; set; }
        /// <summary>
        /// the injected object
        /// </summary>
        public Func<object> GetInjectObj { get; set; }
        /// <summary>
        /// capture keyboard event
        /// </summary>
        public IKeyboardHandler KeyboardHandler { get; set; }
        /// <summary>
        /// capture js dialog event, alert or confirm, default blocking all
        /// </summary>
        public IJsDialogHandler JsDialogHandler { get; set; } = new DefaultJsDialogHandler();
        /// <summary>
        /// life span handler, default capture popup window event and using default app(provided by os) to open it 
        /// </summary>
        public ILifeSpanHandler LifeSpanHandler { get; set; } = new DefaultLifeSpanHandler();
        /// <summary>
        /// drag drop handler, default blocking
        /// </summary>
        public IDragHandler DragHandler { get; set; } = new DefaultDragHandler();
        /// <summary>
        /// context menu handler, default blocking all
        /// </summary>
        public IContextMenuHandler MenuHandler { get; set; } = new ContextMenuHandler();
    }
}

using CefSharp;
using System.Collections.Generic;

namespace CefBox.CefHandlers
{
    public class CefDragHandler : IDragHandler
    {
        private readonly IAppFrame _targetForm;
        public CefDragHandler(IAppFrame target)
        {
            this._targetForm = target;
        }

        public bool OnDragEnter(IWebBrowser browserControl, IBrowser browser, IDragData dragData, DragOperationsMask mask)
        {
            if (dragData.IsFile)
            {
                _targetForm.DraggedFiles = dragData.FileNames;
            }
            return false;
        }

        public void OnDraggableRegionsChanged(IWebBrowser browserControl, IBrowser browser, IList<DraggableRegion> regions)
        {
        }
    }
}

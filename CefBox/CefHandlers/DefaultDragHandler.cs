using CefSharp;
using System.Collections.Generic;

namespace Snail.GT.Service.CEF
{
    class DefaultDragHandler : IDragHandler
    {
        public bool OnDragEnter(IWebBrowser browserControl, IBrowser browser, IDragData dragData, DragOperationsMask mask)
        {
            return true;
        }

        public void OnDraggableRegionsChanged(IWebBrowser browserControl, IBrowser browser, IList<DraggableRegion> regions)
        {
        }
    }
}

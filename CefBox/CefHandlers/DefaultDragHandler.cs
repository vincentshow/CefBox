using CefSharp;
using System.Collections.Generic;

namespace CefBox.Handlers
{
    class DefaultDragHandler : IDragHandler
    {
        /// <summary>
        /// default behavior, forbidden drag
        /// </summary>
        /// <param name="browserControl"></param>
        /// <param name="browser"></param>
        /// <param name="dragData"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public bool OnDragEnter(IWebBrowser browserControl, IBrowser browser, IDragData dragData, DragOperationsMask mask)
        {
            return true;
        }

        public void OnDraggableRegionsChanged(IWebBrowser browserControl, IBrowser browser, IList<DraggableRegion> regions)
        {
        }
    }
}

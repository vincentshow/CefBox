using CefBox.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CefBox.Middlewares
{
    public class FrameMiddleware : IMiddleware
    {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, IAppFrame> framesMapper;
        private IAppFrame _mainFrame;

        public FrameMiddleware(ILoggerFactory factory, IAppFrame mainFrame)
        {
            _logger = factory.CreateLogger(this.GetType());
            _mainFrame = mainFrame;

            framesMapper = new ConcurrentDictionary<string, IAppFrame>();
        }

        public async Task InvokeAsync(AppRequest context, RequestDelegate next)
        {
            context.FrameId = string.IsNullOrEmpty(context.FrameId) ? GlobalConfig.AppOptions.MainFrameId : context.FrameId;
            context.Frame = this.GetFrameById(context.FrameId);

            if (context.Service.Equals("frame", StringComparison.OrdinalIgnoreCase))
            {
                switch (context.Action.Trim().ToLower())
                {
                    case ("open"):
                        Open(context);
                        break;
                    case ("close"):
                        Close(context);
                        break;
                }
            }
            else if (context.Frame == null)
            {
                throw new InvalidOperationException($"frame not found by {context.FrameId}");
            }

            await next(context);
        }

        private IAppFrame GetFrameById(string frameId)
        {
            IAppFrame frame = _mainFrame;
            if (!string.IsNullOrEmpty(frameId) && GlobalConfig.AppOptions.MainFrameId != frameId)
            {
                framesMapper.TryGetValue(frameId, out frame);
            }

            return frame;
        }

        private void Open(AppRequest request)
        {
            var frameOptions = request.Data.ToObject<FrameOptions>();
            if (frameOptions == null)
            {
                throw new ArgumentNullException("frameOptions");
            }

            if (frameOptions.IsMain)
            {
                if (request.Frame != _mainFrame)
                {
                    throw new AppException(ExceptionCode.InvalidOperation, "change main frame must be requested from main frame");
                }
                request.Frame.ResetFrame(frameOptions);
            }
            else
            {
                var frame = this.GetFrameById(frameOptions.Id);
                if (frame != null)
                {
                    frame.ShowFrame();
                    if (!frameOptions.NewFormPerRequest)
                    {
                        frame.ResetFrame(frameOptions);
                    }
                    return;
                }

                this.framesMapper.TryRemove(frameOptions.Id, out frame);

                frame = request.Frame.ShowSubForm(frameOptions);
                this.framesMapper.TryAdd(frameOptions.Id, frame);
            }
        }

        private void Close(AppRequest request)
        {
            request.Frame.BeforeCloing();

            var hideToTray = true;
            if (request.Data != null && request.Data.Count > 0)
            {
                hideToTray = request.Data.Value<bool>("hideToTray");
            }

            IAppFrame frame = null;
            request.Frame.CloseFrame(hideToTray ? CloseTypes.Hide2Tray : CloseTypes.CloseSelf);
            this.framesMapper.TryRemove(request.FrameId, out frame);
        }
    }
}

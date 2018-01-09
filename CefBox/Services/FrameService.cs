using CefBox.Extensions;
using CefBox.Models;
using System.Threading.Tasks;

namespace CefBox.Services
{
    public class FrameService : IAppService
    {
        public async Task Reset(AppRequest request)
        {
            await request.ExecJSCallback(() =>
            {
                var options = request.Data.ToObject<FrameOptions>();
                if (options == null)
                {
                    throw new AppException(ExceptionCode.InvalidArgument, "frame options cannot be null");
                }
                request.Frame.ResetFrame(options);
                return 1;
            });
        }

        public async Task Open(AppRequest request)
        {
            await request.ExecJSCallback(() =>
            {
                return 1;
            });
        }

        public async Task Move(AppRequest request)
        {
            await request.ExecJSCallback(() =>
            {
                request.Frame.MoveFrame();
                return 1;
            });
        }

        public async Task ShowDevTools(AppRequest request)
        {
            await request.ExecJSCallback(() =>
            {
                request.Frame.ShowDevTools();
                return 1;
            });
        }

        public async Task Reload(AppRequest request)
        {
            await request.ExecJSCallback(() =>
            {
                request.Frame.Reload();
                return 1;
            });
        }

        public async Task Minimum(AppRequest request)
        {
            await request.ExecJSCallback(() =>
            {
                request.Frame.Minimum();
                return 1;
            });
        }

        public async Task Maximum(AppRequest request)
        {
            await request.ExecJSCallback(() =>
            {
                request.Frame.Maximum();
                return 1;
            });
        }

        public async Task Close(AppRequest request)
        {
            await request.ExecJSCallback(() =>
            {
                var hideToTray = true;
                if (request.Data != null && request.Data.Count > 0)
                {
                    hideToTray = request.Data.Value<bool>("hideToTray");
                }

                request.Frame.CloseFrame(hideToTray ? CloseTypes.Hide2Tray : CloseTypes.CloseSelf);
                return 1;
            });
        }

    }
}

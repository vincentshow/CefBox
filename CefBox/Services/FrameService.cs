using CefBox.Models;
using System.Threading.Tasks;

namespace CefBox.Services
{
    public class FrameService : IAppService
    {
        public async Task ShowDevTools(AppRequest request)
        {
            await request.ExecJSCallback(() =>
            {
                request.Frame.ShowDevTools();
                return 1;
            });
        }
    }
}

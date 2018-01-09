using CefBox.Extensions;
using CefBox.Models;
using CefSharp;
using CefSharp.Internals;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CefBox.WinForm
{
    public partial class AppFrame : BorderlessForm, IAppFrame
    {
        #region Fields

        private FrameOptions _options = new FrameOptions();
        private Point _lastCentralPoint;

        private ChromiumWebBrowser _localBrowser { get; set; }
        public IWebBrowser Browser
        {
            get { return _localBrowser; }
            set { throw new NotImplementedException(); }
        }

        public IEnumerable<string> DraggedFiles { get; set; }

        private int _localHeaderHeight = 0;

        #endregion

        #region Constructor

        public AppFrame() : this(null)
        {
        }

        public AppFrame(FrameOptions options)
            : base(options?.ShowHeaderBar, AppConfiguration.GetConfig<bool>("debug.showdevtools"), options?.Resizable)
        {
            if (this.DesignMode)
            {
                return;
            }

            //todo dynamic set
            var iconPath = $"app.ico";
            if (File.Exists(iconPath))
            {
                this.Icon = new Icon(iconPath);
            }
            this.BackColor = Color.FromArgb(241, 242, 244);
            this._options = options ?? new FrameOptions();

            Load += (sender, e) =>
            {
                ResetOptions(options);
                this.ReloadConcent = () => this.Reload();
                this.ShowTools = () => this.ShowDevTools();
                this.CloseLocale = () => this.CloseFrame(CloseTypes.CloseSelf);
                this.LoadCEF(this.GetCefOptions(options, injectObj: AppHoster.Instance));
            };
            ResizeEnd += (s, e) => this.ResetCentralPoint();
        }

        #endregion

        #region IAppFrame

        public void Maximum()
        {
            this.InvokeActionSafely(() =>
            {
                base.ToggleMaximize();
            });
        }

        public void Minimum()
        {
            this.InvokeActionSafely(() =>
            {
                this.RestoreBrowser();
                WindowState = FormWindowState.Minimized;
            });
        }

        public void ShowMsg(string msg, ShowMsgTypes type = ShowMsgTypes.Info)
        {
            this.InvokeActionSafely(() =>
            {
                MessageBox.Show(msg);
            });
        }

        public void MoveFrame()
        {
            this.InvokeActionSafely(() =>
            {
                HitMouseDown(WinApi.HitTest.HTCAPTION);
            });
        }

        public virtual void CloseFrame(CloseTypes type = CloseTypes.CloseSelf)
        {
            this.InvokeActionSafely(() =>
            {
                base.Hide();
                this.RestoreBrowser();
            });

            if (type == CloseTypes.Hide2Tray)
            {
                return;
            }

            if (type == CloseTypes.CloseSelf && this._options.IsMain)
            {
                type = CloseTypes.CloseApp;
            }

            if (type == CloseTypes.CloseApp)
            {
                var cancellationTokenSource = new CancellationTokenSource();
                Task.Run(async () =>
                {
                    await Task.Delay(10000);
                    if (!cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        Process.GetCurrentProcess().Kill();
                    }
                }, cancellationTokenSource.Token);

                base.Close();

                CefManager.Shutdown();
                //Application.Exit();
                Environment.Exit(0);

                cancellationTokenSource.Cancel();
            }
            else
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(5000);
                        this?.InvokeActionSafely(() =>
                        {
                            base.Close();
                            this.Browser?.Dispose();
                        });
                    }
                    catch
                    {
                    }
                });
            }
        }

        public void ResetFrame(FrameOptions options)
        {
            this.InvokeActionSafely(() =>
            {
                if (options == null)
                {
                    return;
                }

                var isSameContent = options?.ContentPath == _options?.ContentPath;
                this.ChangeProps(options.Resizable);
                ResetOptions(options);

                if (!isSameContent && !string.IsNullOrEmpty(options.ContentPath))
                {
                    this.LoadCEF(this.GetCefOptions(options, this._options));
                }
            });
        }

        public virtual void ShowFrame()
        {
            this.InvokeActionSafely(() =>
            {
                base.Show();

                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
                this.Activate();

                RestoreBrowser(true);
                this.BringToFront();
            });
        }

        public void ResetMouse(float left, float top)
        {
            left *= DpiRatioX;
            top *= DpiRatioY;

            left += this.Location.X;
            top += this.Location.Y + _localHeaderHeight;

            Cursor.Position = new Point((int)left, (int)top);
        }

        public void Reload()
        {
            this.InvokeActionSafely(() =>
            {
                this._localBrowser?.Reload();
            });
        }

        public void ShowDevTools()
        {
            this.InvokeActionSafely(() =>
            {
                this._localBrowser?.ShowDevTools();
            });
        }

        public IAppFrame ShowSubForm(FrameOptions options)
        {
            AppFrame frame = null;
            this.InvokeActionSafely(() =>
            {
                frame = new AppFrame(options);
                if (options.IsModal)
                {
                    this.Enabled = false;
                    frame.Show(this);
                }
                else
                {
                    frame.Show();
                }
            });
            return frame;
        }

        public void BeforeCloing()
        {
            var parent = this.Owner;
            if (parent != null)
            {
                this.InvokeActionSafely(() =>
                {
                    parent.Enabled = true;
                    parent.Activate();
                    parent.Focus();
                });
            }
        }

        #endregion

        #region private helper

        private void LoadCEF(CefOptions cefOptions, object extendParam = null)
        {
            var _contentPath = string.Empty;
            if (!string.IsNullOrEmpty(cefOptions.ContentPath) && cefOptions.ContentPath.StartsWith("chrome://"))
            {
                _contentPath = cefOptions.ContentPath;
            }
            else
            {
                _contentPath = cefOptions.IsRelativeContent ? Path.Combine(cefOptions.BasePath, cefOptions.ContentPath) : cefOptions.ContentPath;
            }

            if (!cefOptions.IsRelativeContent && !Uri.IsWellFormedUriString(_contentPath, UriKind.RelativeOrAbsolute))
            {
                throw new ArgumentException("invalid content path found", _contentPath);
            }

            var checkPath = _contentPath;
            var param = string.Empty;
            var pathArray = _contentPath.Split('?');
            if (pathArray.Length > 1)
            {
                checkPath = pathArray[0];
                param = $"?{pathArray[1]}";
            }

            //如果本地存在文件，需要将本地路径中的特殊字符转掉，通常发生在debug环境下
            if (File.Exists(checkPath))
            {
                _contentPath = checkPath.ToAppPath() + param;
            }

            if (this._localBrowser == null)
            {
                this.InitLocalBrowser(_contentPath, extendParam);
                CefManager.LoadBrowser(this._localBrowser, cefOptions);
            }
            else if (this._localBrowser.Address != _contentPath)
            {
                this._localBrowser.Load(_contentPath);
            }
        }

        private void InitLocalBrowser(string contentPath, object extendParam)
        {
            if (this._localBrowser != null)
            {
                return;
            }

            this._localBrowser = new ChromiumWebBrowser(contentPath)
            {
                Dock = DockStyle.None,
                FocusHandler = null,
                BackColor = Color.AliceBlue,
                Padding = new Padding(5)
            };

            this.Controls.Add(this._localBrowser);
            this._localBrowser.IsBrowserInitializedChanged += (sender, e) =>
            {
                if (!e.IsBrowserInitialized)
                {
                    return;
                }

                this.InvokeActionSafely(() =>
                {
                    if (extendParam == null)
                    {
                        this.Opacity = 1;
                    }
                    this._localBrowser.Dock = DockStyle.Fill;
                    this.Browser.Focus();
                });
                this.ResetWorkingSet();
            };
        }

        private void SetFormPosition()
        {
            if (this._options.Position == FramePositions.CenterScreen || _lastCentralPoint.IsEmpty)
            {
                this.CenterToParent();
            }
            else
            {
                var location = new Point();
                if (this._options.Position == FramePositions.BottomRight)
                {
                    location.X = Screen.PrimaryScreen.WorkingArea.Width - Width - 10;
                    location.Y = Screen.PrimaryScreen.WorkingArea.Height - Height - 10;
                }
                else
                {
                    location.X = _lastCentralPoint.X - Width / 2;
                    location.Y = Math.Max(0, _lastCentralPoint.Y - Height / 2);
                }

                Location = location;
            }

            if (this._options.Position != FramePositions.RefPreLocation)
            {
                this._lastCentralPoint = new Point(Location.X + Width / 2, Location.Y + Height / 2);
            }
        }

        private void ResetOptions(FrameOptions options, bool includePosition = true)
        {
            if (DesignMode)
            {
                return;
            }

            this.SuspendLayout();

            this._options = options ?? new FrameOptions();

            var miniSize = new SizeF(this._options.MinWidth * DpiRatioX, this._options.MinHeight * DpiRatioY);
            if (miniSize.Width == 0)
            {
                miniSize.Width = this._options.Width * 0.7f * DpiRatioX;
            }
            if (miniSize.Height == 0)
            {
                miniSize.Height = this._options.Height * 0.7f * DpiRatioY;
            }
            this.MinimumSize = miniSize.ToSize();
            this.MaximumSize = new SizeF(Screen.PrimaryScreen.WorkingArea.Width * DpiRatioX, Screen.PrimaryScreen.WorkingArea.Height * DpiRatioY).ToSize();
            this.ClientSize = new SizeF(this._options.Width * DpiRatioX, (this._options.Height + this._localHeaderHeight) * DpiRatioY).ToSize();

            if (includePosition)
            {
                this.SetFormPosition();
            }

            this.Text = options.Name;
            this.ResetCentralPoint();

            this.ResumeLayout();
        }

        private void ResetCentralPoint()
        {
            this._lastCentralPoint = new Point(Location.X + Width / 2, Location.Y + Height / 2);
        }

        private void RestoreBrowser(bool show = false)
        {
            var width = 0;
            var height = 0;
            if (show)
            {
                width = this.Width;
                height = this.Height;
            }
            else
            {
                ResetWorkingSet();
            }
            ResizeBrowser(width, height);
        }

        private void ResizeBrowser(int width = 0, int height = 0)
        {
            if (Browser != null)
            {
                var browserAdapter = ((IWebBrowserInternal)Browser).BrowserAdapter;
                browserAdapter.Resize(width, height);
            }
        }

        private void ResetWorkingSet()
        {
            try
            {
                var random = (IntPtr)750000L;
                Process.GetCurrentProcess().MaxWorkingSet = random;

                var renderName = Path.GetFileNameWithoutExtension(GlobalConfig.AppOptions.RenderName);
                var renders = Process.GetProcessesByName(renderName);
                if (renders != null)
                {
                    foreach (var render in renders)
                    {
                        render.MaxWorkingSet = random;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion
    }

}

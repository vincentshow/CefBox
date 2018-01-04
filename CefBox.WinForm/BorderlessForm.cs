using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CefBox.WinForm
{
    public class BorderlessForm : Form
    {
        private bool _resizable;
        private bool _showDevTools;
        private bool _showHeaderBar;
        private bool _forceShowHeaderbar;
        private System.ComponentModel.IContainer components = null;

        #region layout

        private Label headerTrigger;
        private Panel headerPanel;
        public FlowLayoutPanel contentPanel;

        #endregion

        #region headerbar

        private DateTime titleClickTime = DateTime.MinValue;
        private Point titleClickPosition = Point.Empty;

        private Label ReloadLabel;
        private Label DevToolsLabel;

        private Label TitleLabel;
        private Label MinimizeLabel;
        private Label MaximizeLabel;
        private Label CloseLabel;

        private ToolTip DecorationToolTip;
        private Dictionary<WindowButtons, Label> _winLabelList;

        #endregion

        #region border

        //private SurroundPlaceHolder _placeHolderLeft;
        //private SurroundPlaceHolder _placeHolderTop;
        //private SurroundPlaceHolder _placeHolderRight;
        //private SurroundPlaceHolder _placeHolderBottom;
        private SurroundPlaceHolder _placeHolderRightBottom;
        #endregion

        #region theme

        private Color hoverTextColor = Color.FromArgb(62, 109, 181);
        public Color HoverTextColor
        {
            get { return hoverTextColor; }
            set { hoverTextColor = value; }
        }

        private Color downTextColor = Color.FromArgb(25, 71, 138);
        public Color DownTextColor
        {
            get { return downTextColor; }
            set { downTextColor = value; }
        }

        private Color hoverBackColor = Color.FromArgb(213, 225, 242);
        public Color HoverBackColor
        {
            get { return hoverBackColor; }
            set { hoverBackColor = value; }
        }

        private Color downBackColor = Color.FromArgb(163, 189, 227);
        public Color DownBackColor
        {
            get { return downBackColor; }
            set { downBackColor = value; }
        }

        private Color normalBackColor = Color.White;
        public Color NormalBackColor
        {
            get { return normalBackColor; }
            set { normalBackColor = value; }
        }

        private Color activeTextColor = Color.FromArgb(68, 68, 68);
        public Color ActiveTextColor
        {
            get { return activeTextColor; }
            set { activeTextColor = value; }
        }

        private Color inactiveTextColor = Color.FromArgb(177, 177, 177);
        public Color InactiveTextColor
        {
            get { return inactiveTextColor; }
            set { inactiveTextColor = value; }
        }

        public enum MouseState
        {
            Normal,
            Hover,
            Down
        }
        #endregion

        #region logic

        private int _borderWidth = 12;
        public int HeaderBarHeight
        {
            get
            {
                return _showHeaderBar ? 25 : 0;
            }
        }

        public float DpiRatioX { get; private set; }
        public float DpiRatioY { get; private set; }

        public Action ReloadConcent;
        public Action ShowTools;
        public Action CloseLocale;
        #endregion

        public BorderlessForm() : this(null, null, null)
        {
        }

        public BorderlessForm(bool? showHeaderBar = null, bool? showDevTools = null, bool? resizable = null, bool minimizeBox = true)
        {

            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;

            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                DpiRatioX = graphics.DpiX / 96;
                DpiRatioY = graphics.DpiY / 96;
            }

            this._showDevTools = showDevTools ?? false;
            this._forceShowHeaderbar = showHeaderBar ?? false;
            this._showHeaderBar = _forceShowHeaderbar || this._showDevTools;

            this.InitComponents();
            this.ChangeProps(resizable, minimizeBox);
        }

        public void ChangeProps(bool? resizable = null, bool minimizeBox = true)
        {
            this._resizable = resizable ?? false;
            this.MaximizeBox = resizable.HasValue && resizable.Value;
            this.MinimizeBox = minimizeBox;

            //this.SuspendLayout();

            if (_resizable)
            {
                this.InitBorder();
                this.BindEvent2Border();
            }

            //this.ResumeLayout();
        }

        public void HitMouseDown(MouseEventArgs e, WinApi.HitTest hit)
        {
            if (e.Button == MouseButtons.Left)
            {
                HitMouseDown(hit);
            }
        }

        public void HitMouseDown(WinApi.HitTest hit)
        {
            WinApi.ReleaseCapture();
            WinApi.SendMessage(Handle, (int)WinApi.Messages.WM_NCLBUTTONDOWN, (int)hit, 0);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            //if (m.Msg == (int)WinApi.Messages.WM_NCCALCSIZE)
            //{
            //    if (WindowState == FormWindowState.Maximized)
            //    {
            //        var r = (WinApi.RECT)Marshal.PtrToStructure(m.LParam, typeof(WinApi.RECT));
            //        var x = WinApi.GetSystemMetrics(WinApi.NativeConstants.SM_CXSIZEFRAME);
            //        var y = WinApi.GetSystemMetrics(WinApi.NativeConstants.SM_CYSIZEFRAME);
            //        var p = WinApi.GetSystemMetrics(WinApi.NativeConstants.SM_CXPADDEDBORDER);
            //        var w = x + p;
            //        var h = y + p;

            //        r.left += w;
            //        r.top += h;
            //        r.right -= w;
            //        r.bottom -= h;

            //        var appBarData = new WinApi.APPBARDATA();
            //        appBarData.cbSize = Marshal.SizeOf(typeof(WinApi.APPBARDATA));
            //        var autohide = (WinApi.SHAppBarMessage(WinApi.NativeConstants.ABM_GETSTATE, ref appBarData) & WinApi.NativeConstants.ABS_AUTOHIDE) != 0;
            //        if (autohide) r.bottom -= 1;

            //        Marshal.StructureToPtr(r, m.LParam, true);
            //    }
            //    m.Result = IntPtr.Zero;
            //    return;
            //}

            //if (m.Msg == (int)WinApi.Messages.WM_WINDOWPOSCHANGED)
            //{
            //    DefWndProc(ref m);
            //    UpdateBounds();
            //    m.Result = new IntPtr(1);
            //    return;
            //}

            if (m.Msg == (int)WinApi.Messages.WM_NCPAINT)
            {
                var v = 2;
                WinApi.DwmSetWindowAttribute(this.Handle, WinApi.DWMWINDOWATTRIBUTE.NCRenderingPolicy, ref v, 4);

                WinApi.Margins margins = new WinApi.Margins()
                {
                    bottomHeight = 1,
                    leftWidth = 1,
                    rightWidth = 1,
                    topHeight = 1
                };
                WinApi.DwmExtendFrameIntoClientArea(this.Handle, ref margins);
            }

            if (m.Msg == (int)WinApi.Messages.WM_COPYDATA)
            {
                var copyData = (WinApi.CopyData)Marshal.PtrToStructure(m.LParam, typeof(WinApi.CopyData));
                var bt = new byte[copyData.cbData];

                Marshal.Copy(copyData.lpData, bt, 0, bt.Length);

                var data = Encoding.Default.GetString(bt);
                if (data == "0")
                {
                    if (CloseLocale != null)
                    {
                        CloseLocale();
                    }
                    else
                    {
                        Close();
                    }
                }
            }

        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x00020000;   // 允许点击任务栏最小化窗口
                return cp;
            }
        }

        #region private helper

        private void InitComponents()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;

            this.SuspendLayout();

            if (_showHeaderBar)
            {
                this.InitHeaderBar();
                this.BindEvent2HeaderBar();
            }

            SizeChanged += BorderlessForm_SizeChanged;
            Activated += BorderlessForm_Activated;

            this.ResumeLayout(false);
        }

        private void InitHeaderBar()
        {
            this._winLabelList = new Dictionary<WindowButtons, Label>();
            this.DecorationToolTip = new ToolTip(this.components);
            this.headerTrigger = new Label
            {
                Text = "=====",
                BackColor = Color.Black,
                ForeColor = Color.GhostWhite,
                Anchor = AnchorStyles.Top,
                Height = 15,
                Width = 40,
                Top = 0,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            this.headerPanel = new Panel
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(0, 0),
                Size = new Size(this.Width, this.HeaderBarHeight)
            };
            this.headerPanel.SuspendLayout();

            var Webdings = new Font("Webdings", 10f);

            if (this._showDevTools)
            {
                this.ReloadLabel = new Label();
                this.ReloadLabel.Font = Webdings;
                this.ReloadLabel.BackColor = Color.White;
                this.ReloadLabel.Name = "ReloadLabel";
                this.ReloadLabel.Size = new Size(25, 25);
                this.ReloadLabel.Text = "q";
                this.ReloadLabel.TextAlign = ContentAlignment.MiddleCenter;
                this.DecorationToolTip.SetToolTip(this.ReloadLabel, "Reload");
                this._winLabelList.Add(WindowButtons.Reload, this.ReloadLabel);

                this.DevToolsLabel = new Label();
                this.DevToolsLabel.Font = Webdings;
                this.DevToolsLabel.BackColor = Color.White;
                this.DevToolsLabel.Name = "ToolsLabel";
                this.DevToolsLabel.Size = new Size(25, 25);
                this.DevToolsLabel.Text = "@";
                this.DevToolsLabel.TextAlign = ContentAlignment.MiddleCenter;
                this.DecorationToolTip.SetToolTip(this.DevToolsLabel, "DevTools");
                this._winLabelList.Add(WindowButtons.DevTools, this.DevToolsLabel);
            }

            if (this._resizable)
            {
                this.MaximizeLabel = new Label();
                this.MaximizeLabel.Font = Webdings;
                this.MaximizeLabel.BackColor = Color.White;
                this.MaximizeLabel.Name = "MaximizeLabel";
                this.MaximizeLabel.Size = new Size(25, 25);
                this.MaximizeLabel.Text = "1";
                this.MaximizeLabel.TextAlign = ContentAlignment.MiddleCenter;
                this.DecorationToolTip.SetToolTip(this.MaximizeLabel, "Maximize");
                this._winLabelList.Add(WindowButtons.Maximize, this.MaximizeLabel);
            }

            if (this.MinimizeBox)
            {
                this.MinimizeLabel = new Label();
                this.MinimizeLabel.Font = Webdings;
                this.MinimizeLabel.BackColor = Color.White;
                this.MinimizeLabel.Name = "MinimizeLabel";
                this.MinimizeLabel.Size = new Size(25, 25);
                this.MinimizeLabel.Text = "0";
                this.MinimizeLabel.TextAlign = ContentAlignment.MiddleCenter;
                this.DecorationToolTip.SetToolTip(this.MinimizeLabel, "Minimize");
                this._winLabelList.Add(WindowButtons.Minimize, this.MinimizeLabel);
            }

            this.CloseLabel = new Label();
            this.CloseLabel.Font = Webdings;
            this.CloseLabel.BackColor = Color.White;
            this.CloseLabel.Name = "CloseLabel";
            this.CloseLabel.Size = new Size(25, 25);
            this.CloseLabel.Text = "r";
            this.CloseLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.DecorationToolTip.SetToolTip(this.CloseLabel, "Close");
            this._winLabelList.Add(WindowButtons.Close, this.CloseLabel);

            this.TitleLabel = new Label();
            this.TitleLabel.BackColor = Color.White;
            this.TitleLabel.Padding = new Padding(5, 0, 0, 0);
            this.TitleLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.TitleLabel.ForeColor = Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Height = 25;
            this.TitleLabel.Location = new Point(0, 0);
            this.TitleLabel.Text = "Title";
            this.TitleLabel.TextAlign = ContentAlignment.MiddleLeft;

            var orderedPairs = _winLabelList.OrderBy(pair => pair.Key);
            foreach (var pair in orderedPairs)
            {
                this.headerPanel.Controls.Add(pair.Value);
            }
            this.headerPanel.Controls.Add(this.TitleLabel);

            this.Controls.Add(this.headerTrigger);
            this.Controls.Add(this.headerPanel);

            this.headerPanel.PerformLayout();
            if (!_forceShowHeaderbar)
            {
                this.headerPanel.Hide();
            }
            else
            {
                this.headerTrigger.Hide();
            }
        }

        private void BindEvent2HeaderBar()
        {
            foreach (var pair in _winLabelList)
            {
                pair.Value.MouseEnter += (s, e) => SetLabelColors((Control)s, MouseState.Hover);
                pair.Value.MouseLeave += (s, e) => SetLabelColors((Control)s, MouseState.Normal);
                pair.Value.MouseDown += (s, e) => SetLabelColors((Control)s, MouseState.Down);

                if (pair.Key == WindowButtons.Minimize)
                {
                    pair.Value.MouseClick += (s, e) =>
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            WindowState = FormWindowState.Minimized;
                        }
                    };
                }

                if (pair.Key == WindowButtons.Maximize)
                {
                    pair.Value.MouseClick += (s, e) =>
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            ToggleMaximize();
                        }
                    };
                }

                if (pair.Key == WindowButtons.Close)
                {
                    pair.Value.MouseClick += (s, e) =>
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            if (CloseLocale != null)
                            {
                                CloseLocale();
                            }
                            else
                            {
                                Close();
                            }
                        }
                    };
                }

                if (pair.Key == WindowButtons.Reload)
                {
                    pair.Value.MouseClick += (s, e) =>
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            ReloadConcent?.Invoke();
                        }
                    };
                }

                if (pair.Key == WindowButtons.DevTools)
                {
                    pair.Value.MouseClick += (s, e) =>
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            ShowTools?.Invoke();
                        }
                    };
                }
            }

            TitleLabel.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    var clickTime = (DateTime.Now - titleClickTime).TotalMilliseconds;
                    if (clickTime < SystemInformation.DoubleClickTime && e.Location == titleClickPosition)
                    {
                        ToggleMaximize();
                    }
                    else
                    {
                        titleClickTime = DateTime.Now;
                        titleClickPosition = e.Location;
                        HitMouseDown(e, WinApi.HitTest.HTCAPTION);
                    }
                }
            };

            TitleLabel.MouseDoubleClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ToggleMaximize();
                }
            };

            TitleLabel.Text = Text;
            TextChanged += (s, e) => TitleLabel.Text = Text;

            if (!this._forceShowHeaderbar)
            {
                this.headerTrigger.MouseHover += (s, e) =>
                {
                    this.headerTrigger.Hide();
                    this.headerPanel.Show();
                };

                foreach (Control control in this.headerPanel.Controls)
                {
                    control.MouseEnter += (s, e) => this.ToggleHeaderbar(true);
                    control.MouseLeave += (s, e) =>
                    {
                        var mousePos = PointToClient(MousePosition);
                        if (mousePos.X < 0 || mousePos.X > this.Width || mousePos.Y > this.HeaderBarHeight || mousePos.Y < 0)
                        {
                            this.ToggleHeaderbar(false);
                        }
                    };
                }
            }
        }

        private void ToggleHeaderbar(bool show)
        {
            this.headerPanel.Visible = show;
            this.headerTrigger.Visible = !show;
        }

        public void ToggleMaximize()
        {
            if (!this._resizable)
            {
                return;
            }

            var labelText = string.Empty;
            if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Maximized;
                labelText = "2";
            }
            else
            {
                WindowState = FormWindowState.Normal;
                labelText = "1";
            }

            if (this.MaximizeLabel != null)
            {
                this.MaximizeLabel.Text = labelText;
            }
        }

        private void InitBorder()
        {
            //this._placeHolderLeft = new SurroundPlaceHolder();
            //this.Controls.Add(this._placeHolderLeft);

            //this._placeHolderTop = new SurroundPlaceHolder();
            //this.Controls.Add(this._placeHolderTop);

            //this._placeHolderRight = new SurroundPlaceHolder();
            //this._placeHolderRight.BringToFront();
            //this.Controls.Add(this._placeHolderRight);

            //this._placeHolderBottom = new SurroundPlaceHolder();
            //this.Controls.Add(this._placeHolderBottom);

            this._placeHolderRightBottom = new SurroundPlaceHolder(5, true);
            this._placeHolderRightBottom.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            this._placeHolderRightBottom.Cursor = Cursors.SizeNWSE;
            this.Controls.Add(this._placeHolderRightBottom);
            //this._placeHolderLeft.MouseMove += PlaceHolder_MouseMove;
            //this._placeHolderLeft.MouseDown += PlaceHolder_MouseDown;

            //this._placeHolderTop.MouseMove += PlaceHolder_MouseMove;
            //this._placeHolderTop.MouseDown += PlaceHolder_MouseDown;

            //this._placeHolderRight.MouseMove += PlaceHolder_MouseMove;
            //this._placeHolderRight.MouseDown += PlaceHolder_MouseDown;

            //this._placeHolderBottom.MouseMove += PlaceHolder_MouseMove;
            //this._placeHolderBottom.MouseDown += PlaceHolder_MouseDown;

        }

        private void BindEvent2Border()
        {
            this._placeHolderRightBottom.MouseDown += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    HitMouseDown(e, WinApi.HitTest.HTBOTTOMRIGHT);
                }
            };
        }

        private void BorderlessForm_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                return;
            }

            if (this._resizable)
            {
                SetBorderPosition();
            }

            if (this._showHeaderBar)
            {
                SetWinButtonPosition();
            }
        }

        private void BorderlessForm_Activated(object sender, EventArgs e)
        {
            if (this.HeaderBarHeight == 0)
            {
                return;
            }

            foreach (var control in _winLabelList.Values)
            {
                SetLabelColors(control, MouseState.Normal);
            }
        }

        private void SetBorderPosition()
        {
            if (this._placeHolderRightBottom == null)
            {
                return;
            }

            this._placeHolderRightBottom.Visible = WindowState != FormWindowState.Maximized;

            //this._placeHolderLeft.Location = new Point(0, 0);
            //this._placeHolderLeft.Size = new Size(_borderWidth, Height);

            //this._placeHolderTop.Location = new Point(_borderWidth, 0);
            //this._placeHolderTop.Size = new Size(Width - _borderWidth, _borderWidth);

            //this._placeHolderRight.BringToFront();
            //this._placeHolderRight.Location = new Point(Width - _borderWidth, _borderWidth);
            //this._placeHolderRight.Size = new Size(_borderWidth, Height - _borderWidth);

            //this._placeHolderBottom.BringToFront();
            //this._placeHolderBottom.Location = new Point(_borderWidth, Height - _borderWidth);
            //this._placeHolderBottom.Size = new Size(Width - 2 * _borderWidth, _borderWidth);

            this._placeHolderRightBottom.BringToFront();
            this._placeHolderRightBottom.Location = new Point(Width - _borderWidth - 2, Height - _borderWidth - 2);
            this._placeHolderRightBottom.Size = new Size(_borderWidth, _borderWidth);
        }

        protected void SetLabelColors(Control control, MouseState state)
        {
            if (!ContainsFocus) return;

            var textColor = ActiveTextColor;
            var backColor = NormalBackColor;

            switch (state)
            {
                case MouseState.Hover:
                    textColor = HoverTextColor;
                    backColor = HoverBackColor;
                    break;
                case MouseState.Down:
                    textColor = DownTextColor;
                    backColor = DownBackColor;
                    break;
            }

            control.ForeColor = textColor;
            control.BackColor = backColor;
        }

        private void SetWinButtonPosition()
        {
            var orderedPairs = _winLabelList.OrderBy(pair => pair.Key);
            var lastLabelX = 25;
            foreach (var pair in orderedPairs)
            {
                pair.Value.Left = this.Width - lastLabelX;
                lastLabelX += 25;
            }

            this.TitleLabel.Width = this.Width - lastLabelX + 25;
            this.headerPanel.Width = this.Width;

            if (!_forceShowHeaderbar)
            {
                headerTrigger.Left = (this.Width - headerTrigger.Width) / 2;
            }
        }

        private enum WindowButtons
        {
            Close,
            Maximize,
            Minimize,
            DevTools,
            Reload
        }
        #endregion
    }
}

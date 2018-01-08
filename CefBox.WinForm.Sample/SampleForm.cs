using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CefBox.WinForm.Sample
{
    public partial class SampleForm : AppFrame
    {
        public SampleForm()
            : base(new FrameOptions
            {
                ContentPath = "index.html",
                Height = 500,
                Width = 700,
                IsMain = true
            })
        {
            this.Icon = new Icon("chromium.ico");
            InitializeComponent();
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.ShowForm();
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.CloseForm(CloseTypes.CloseSelf);
        }

        private void ShowWindow_Click(object sender, EventArgs e)
        {
            this.ShowForm();
        }

        private void Hello_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello from CefBox");
        }

    }
}

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
                Height = 600,
                Width = 800,
                IsMain = true
            })
        {
            InitializeComponent();
            FormClosing += SampleForm_FormClosing;
        }

        private void SampleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            this.WindowState = FormWindowState.Minimized;
            this.Hide();

            this.notifyIcon.Visible = false;
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.ShowFrame();
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.CloseFrame(CloseTypes.CloseSelf);
        }

        private void ShowWindow_Click(object sender, EventArgs e)
        {
            this.ShowFrame();
        }

        private void Hello_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello from CefBox");
        }

    }
}

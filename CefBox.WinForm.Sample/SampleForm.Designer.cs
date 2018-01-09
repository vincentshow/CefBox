using System;
using System.Drawing;
using System.Windows.Forms;

namespace CefBox.WinForm.Sample
{
    partial class SampleForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.login = new System.Windows.Forms.ToolStripMenuItem();
            this.showWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.exit = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();

            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.login,
            this.showWindow,
            this.exit});
            this.contextMenuStrip.Name = "contextMenuStrip";
            // 
            // hello
            // 
            this.login.Name = "hello";
            this.login.Text = "Hello";
            this.login.Click += new System.EventHandler(this.Hello_Click);
            // 
            // showWindow
            // 
            this.showWindow.Name = "showWindow";
            this.showWindow.Text = "Show";
            this.showWindow.Click += new System.EventHandler(this.ShowWindow_Click);
            // 
            // exit
            // 
            this.exit.Name = "exit";
            this.exit.Image = new Bitmap("app.ico");
            this.exit.Text = "Exit";
            this.exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;

            this.notifyIcon.Icon = this.Icon;
            this.notifyIcon.Text = "CefBox Sample";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseClick);

            // 
            // SampleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Location = new System.Drawing.Point(0, 0);

            this.Name = "SampleForm";
            this.Text = "CefBox Sample";
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ToolStripMenuItem login;
        private System.Windows.Forms.ToolStripMenuItem showWindow;
        private System.Windows.Forms.ToolStripMenuItem exit;
    }
}


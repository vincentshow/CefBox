﻿using System;
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
    public partial class Form1 : AppFrame
    {
        public Form1() : base(new FrameOptions
        {
            ContentPath = "index.html",
            Height = 500,
            Width = 700,
            IsMain = true
        })
        {
            InitializeComponent();
        }
    }
}

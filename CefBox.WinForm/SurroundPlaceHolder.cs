using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CefBox.WinForm
{
    class SurroundPlaceHolder : Control
    {
        private readonly int _step;
        private readonly bool _isTriangle;
        public SurroundPlaceHolder(int step = 1, bool isTriangle = false)
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            //this.BackColor = Color.Black;
            this.BackColor = Color.Transparent;
            this._step = step;
            this._isTriangle = isTriangle;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            //Graphics g = pevent.Graphics;

            ////if (!_isTriangle)
            ////{
            ////    //g.DrawRectangle(Pens.Red, this.ClientRectangle);
            ////    g.DrawRectangle(Pens.Transparent, this.ClientRectangle);
            ////}
            ////else
            ////{
            ////    var baseX = this.Location.X + this.Width;
            ////    var baseY = this.Location.Y + this.Height;
            ////    var pen = new Pen(Color.Gray, 1);

            ////    g.DrawLine(pen, 0, Height, Width, 0);
            ////    g.DrawLine(pen, _step, Height, Width, _step);
            ////    g.DrawLine(pen, 2 * _step, Height, Width, 2 * _step);
            ////}

            //this.BringToFront();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // don't call the base class
            //base.OnPaintBackground(pevent);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_TRANSPARENT = 0x20;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TRANSPARENT;
                return cp;
            }
        }
    }
}

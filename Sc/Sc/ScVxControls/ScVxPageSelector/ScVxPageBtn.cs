using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{ 
    public class ScVxPageBtn :ScLayer
    {

        public float ratio = 0;
        public Color fillColor = Color.FromArgb(119, 175, 188);
        public Color mouseEnterFillColor = Color.LightGray;
        public Color frameColor = Color.FromArgb(119, 175, 188);
        public Color mouseEnterFrameColor = Color.DarkGray;
        public float frameWidth = 1f;

        Color curtFillColor = Color.FromArgb(119, 175, 188);
        Color curtFrameColor = Color.FromArgb(119, 175, 188);

        int state = 0;

        public ScVxPageBtn()
        {
            IsUseHitGeometryLayerBound = true;
            GDIPaint += ScVxPageBtn_GDIPaint;

            MouseEnter += ScVxPageBtn_MouseEnter;
            MouseLeave += ScVxPageBtn_MouseLeave;
            MouseDown += ScVxPageBtn_MouseDown;
        }

        private void ScVxPageBtn_MouseDown(object sender, ScMouseEventArgs e)
        {
           // ratio = 1.0f;
          //  Refresh();
        }

        private void ScVxPageBtn_MouseLeave(object sender)
        {
            state = 0;
            curtFrameColor = frameColor;
            curtFillColor = fillColor;
            Refresh();
        }

        private void ScVxPageBtn_MouseEnter(object sender, ScMouseEventArgs e)
        {
            state = 1;
            curtFrameColor = mouseEnterFrameColor;
            curtFillColor = mouseEnterFillColor;
            Refresh();
        }

        protected override GraphicsPath CreateHitGeometryByGDIPLUS(GDIGraphics g)
        {
            RectangleF rect = new RectangleF(0, 0, Width - 1, Height - 1);
            GraphicsPath circle = new GraphicsPath();
            circle.AddEllipse(rect);
            circle.CloseFigure();
            return circle;
        }

        private void ScVxPageBtn_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;

            RectangleF rect = new RectangleF(0, 0, Width - 1, Height - 1);
            float xoffset;

            if(ratio < 0)
                xoffset = -(1 + ratio) * rect.Width; 
            else
                xoffset = (1 - ratio) * rect.Width;

            RectangleF leftRect = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
            leftRect.X += xoffset;

            Brush brush = new SolidBrush(curtFillColor);
            graphis.FillEllipse(brush, leftRect);
            brush.Dispose();

            if (state == 1)
            {
                brush = new SolidBrush(curtFillColor);
                graphis.FillEllipse(brush, rect);
                brush.Dispose();
            }
   
            //
            Pen pen = new Pen(curtFrameColor, frameWidth);
            graphis.DrawEllipse(pen, rect);
            pen.Dispose();   
        }
    }
}

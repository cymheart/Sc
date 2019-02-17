using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    public class ScTabHeaderPevNextBtn : ScLayer
    {
        ScPanel prePlane;
        ScPanel nextPlane;

        int preBtnMouseState = 0;
        int nextBtnMouseState = 0;


        public delegate void PevOrNextBtnUpEventHandler();
        public event PevOrNextBtnUpEventHandler PevOrNextBtnUpEvent = null;

        public delegate void PevBtnDownEventHandler();
        public event PevBtnDownEventHandler PevBtnDownEvent = null;

        public delegate void NextBtnDownEventHandler();
        public event NextBtnDownEventHandler NextBtnDownEvent = null;

        public ScTabHeaderPevNextBtn(ScMgr scMgr)
            : base(scMgr)
        {
            prePlane = new ScPanel();       
            Add(prePlane);

            nextPlane = new ScPanel();  
            Add(nextPlane);

            prePlane.D2DPaint += PrePlane_D2DPaint;

            prePlane.MouseEnter += PrePlane_MouseEnter;
            prePlane.MouseDown += PrePlane_MouseDown;
            prePlane.MouseLeave += PrePlane_MouseLeave;
            prePlane.MouseUp += PrePlane_MouseUp;

            nextPlane.D2DPaint += NextPlane_D2DPaint;

            nextPlane.MouseEnter += NextPlane_MouseEnter;
            nextPlane.MouseDown += NextPlane_MouseDown;
            nextPlane.MouseLeave += NextPlane_MouseLeave;
            nextPlane.MouseUp += NextPlane_MouseUp;

            SizeChanged += ScTabHeaderPevNextBtn_SizeChanged;

        }

        private void ScTabHeaderPevNextBtn_SizeChanged(object sender, SizeF oldSize)
        {
            float btnWidth = Width / 2 - 1;
            float btnHeight = Height;

            prePlane.Width = btnWidth;
            prePlane.Height = Height;

            nextPlane.Width = btnWidth;
            nextPlane.Height = Height;
            nextPlane.Location = new PointF(Width / 2 + 1, 0);
        }

        private void PrePlane_MouseUp(object sender, ScMouseEventArgs e)
        {
            preBtnMouseState = 1;

            if(PevOrNextBtnUpEvent != null)
                PevOrNextBtnUpEvent();

            prePlane.Refresh();
        }

        private void PrePlane_MouseLeave(object sender)
        {
            preBtnMouseState = 0;
            prePlane.Refresh();
        }

        private void PrePlane_MouseDown(object sender, ScMouseEventArgs e)
        {
            preBtnMouseState = 2;
            prePlane.Refresh();

            if(PevBtnDownEvent != null)
                PevBtnDownEvent();
        }

        private void PrePlane_MouseEnter(object sender, ScMouseEventArgs e)
        {
            preBtnMouseState = 1;
            prePlane.Refresh();
        }

        private void NextPlane_MouseUp(object sender, ScMouseEventArgs e)
        {
            nextBtnMouseState = 1;
            nextPlane.Refresh();

            if (PevOrNextBtnUpEvent != null)
                PevOrNextBtnUpEvent();
        }

        private void NextPlane_MouseLeave(object sender)
        {
            nextBtnMouseState = 0;
            nextPlane.Refresh();
        }

        private void NextPlane_MouseDown(object sender, ScMouseEventArgs e)
        {
            nextBtnMouseState = 2;
            nextPlane.Refresh();

            if (NextBtnDownEvent != null)
                PevBtnDownEvent();
        }

        private void NextPlane_MouseEnter(object sender, ScMouseEventArgs e)
        {
            nextBtnMouseState = 1;
            nextPlane.Refresh();
        }


        private void NextPlane_D2DPaint(D2DGraphics g)
        {
            RawRectangleF rect = new RawRectangleF(0, 0, nextPlane.Width, nextPlane.Height);

            Color color = Color.Black;
            Color orgColor = Color.FromArgb(158, 105, 7);

            switch (nextBtnMouseState)
            {
                case 0:
                    color = Color.FromArgb(200, orgColor);
                    break;

                case 1:
                    color = Color.FromArgb(100, orgColor);
                    break;

                case 2:
                    color = Color.FromArgb(50, orgColor);
                    break;
            }

            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(color);
            SharpDX.Direct2D1.Brush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.FillRectangle(rect, brush);
        }

        private void PrePlane_D2DPaint(D2DGraphics g)
        {
            RawRectangleF rect = new RawRectangleF(0, 0, prePlane.Width, prePlane.Height);

            Color color = Color.Black;
            Color orgColor = Color.FromArgb(158, 105, 7);

            switch (preBtnMouseState)
            {
                case 0:
                    color = Color.FromArgb(200, orgColor);
                    break;

                case 1:
                    color = Color.FromArgb(100, orgColor);
                    break;

                case 2:
                    color = Color.FromArgb(50, orgColor);
                    break;
            }


            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(color);
            SharpDX.Direct2D1.Brush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.FillRectangle(rect, brush);
        }
    }
}

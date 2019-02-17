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
    public class ScTabPreNexBtn:ScLayer
    {
        ScPanel prePlane;
        ScPanel nextPlane;

        public ScTabHead scTabHead;
        int preBtnMouseState = 0;
        int nextBtnMouseState = 0;
        public ScTabPreNexBtn(ScMgr scMgr, float w, float h)
            :base(scMgr)
        {
            Width = w;
            Height = h;

            float btnWidth = w / 2 - 1;
            float btnHeight = h;

            prePlane = new ScPanel();
            prePlane.Width = btnWidth;
            prePlane.Height = h;
            prePlane.Location = new Point(0, 0);
            Add(prePlane);
            
            nextPlane = new ScPanel();
            nextPlane.Width = btnWidth;
            nextPlane.Height = h;
            nextPlane.Location = new PointF(w/2 + 1, 0);
            Add(nextPlane);

            prePlane.D2DPaint += PrePlane_D2DPaint;
            prePlane.GDIPaint += PrePlane_GDIPaint;

            prePlane.MouseEnter += PrePlane_MouseEnter;
            prePlane.MouseDown += PrePlane_MouseDown;
            prePlane.MouseLeave += PrePlane_MouseLeave;
            prePlane.MouseUp += PrePlane_MouseUp;

            nextPlane.D2DPaint += NextPlane_D2DPaint;
            nextPlane.GDIPaint += NextPlane_GDIPaint;

            nextPlane.MouseEnter += NextPlane_MouseEnter;
            nextPlane.MouseDown += NextPlane_MouseDown;
            nextPlane.MouseLeave += NextPlane_MouseLeave;
            nextPlane.MouseUp += NextPlane_MouseUp;

        }

       

        private void PrePlane_MouseUp(object sender, ScMouseEventArgs e)
        {
            preBtnMouseState = 1;
            prePlane.Refresh();

            scTabHead.PreOrNextBtnUp();
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

            scTabHead.PreBtnDown();
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
            scTabHead.PreOrNextBtnUp();
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

            scTabHead.NextBtnDown();
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

        private void NextPlane_GDIPaint(GDIGraphics g)
        {
            Graphics gdiGraph = g.GdiGraph;
            gdiGraph.SmoothingMode = SmoothingMode.Default;

            //   g.SmoothingMode = SmoothingMode.HighQuality;// 指定高质量、低速度呈现。
            //  g.TextRenderingHint = TextRenderingHint.AntiAlias;
            RectangleF rect = new RectangleF(0, 0, nextPlane.Width, nextPlane.Height);

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


            System.Drawing.Brush brush = new SolidBrush(color);
            gdiGraph.FillRectangle(brush, rect);
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

        private void PrePlane_GDIPaint(GDIGraphics g)
        {
            Graphics gdiGraph = g.GdiGraph;
            gdiGraph.SmoothingMode = SmoothingMode.Default;
            //    g.SmoothingMode = SmoothingMode.HighQuality;// 指定高质量、低速度呈现。
            // g.TextRenderingHint = TextRenderingHint.AntiAlias;
            RectangleF rect = new RectangleF(0, 0, prePlane.Width, prePlane.Height);

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


            System.Drawing.Brush brush = new SolidBrush(color);
            gdiGraph.FillRectangle(brush, rect);
        }
    }
}

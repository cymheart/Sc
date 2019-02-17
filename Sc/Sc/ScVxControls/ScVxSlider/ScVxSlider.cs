using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    public class ScVxSlider :ScLayer
    {
        float downWrapperX;
        float downWrapperLocationX;

        float downWrapperY;
        float downWrapperLocationY;

        ScLayer contentLayer;

        ScLayer leftShadowLayer;
        ScLayer rightShadowLayer;
        ScLayer topShadowLayer;
        ScLayer bottomShadowLayer;

        public bool canHorSlider = true;
        public bool canVerSlider = false;

        public bool useHorShadow = true;
        public bool useVerShadow = false;

        public ScVxSlider(ScLayer contentLayer)
        {
            this.contentLayer = contentLayer;
            contentLayer.Location = new PointF(0, 0);
  
            Add(contentLayer);

            leftShadowLayer = new ScLayer();
            rightShadowLayer = new ScLayer();

            topShadowLayer = new ScLayer();
            bottomShadowLayer = new ScLayer();

            leftShadowLayer.GDIPaint += LeftShadowLayer_GDIPaint;
            rightShadowLayer.GDIPaint += RightShadowLayer_GDIPaint;
            topShadowLayer.GDIPaint += TopShadowLayer_GDIPaint;
            bottomShadowLayer.GDIPaint += BottomShadowLayer_GDIPaint;

            Add(leftShadowLayer);
            Add(rightShadowLayer);

            Add(topShadowLayer);
            Add(bottomShadowLayer);

            MouseMove += Slider_MouseMove;
            MouseDown += Slider_MouseDown;
        }

       
        private void TopShadowLayer_GDIPaint(GDIGraphics g)
        {
            if (!useVerShadow)
                return;

            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;

            float topRichHeight = Math.Max(0, 0 - contentLayer.Location.Y);
            RectangleF r = new RectangleF(0, 0, topShadowLayer.Width - 1, topShadowLayer.Height - 2);

            int alpha = (int)((topRichHeight / contentLayer.Height) * 200f);
            LinearGradientBrush brush = new LinearGradientBrush(r, Color.FromArgb(alpha, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), LinearGradientMode.Vertical);
            graphis.FillRectangle(brush, r);
            brush.Dispose();
        }

        private void BottomShadowLayer_GDIPaint(GDIGraphics g)
        {
            if (!useVerShadow)
                return;

            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;

            float bottomRichHeight = Math.Max(0, contentLayer.Location.Y + contentLayer.Height - Height);
            RectangleF r = new RectangleF(0, -2, bottomShadowLayer.Width - 1, bottomShadowLayer.Height);

            int alpha = (int)((bottomRichHeight / contentLayer.Height) * 200f);
            LinearGradientBrush brush = new LinearGradientBrush(r, Color.FromArgb(0, 0, 0, 0), Color.FromArgb(alpha, 0, 0, 0), LinearGradientMode.Vertical);
            graphis.FillRectangle(brush, r);
            brush.Dispose();
        }

        private void RightShadowLayer_GDIPaint(GDIGraphics g)
        {
            if (!useHorShadow)
                return;

            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;

            float rightRichWidth = Math.Max(0, contentLayer.Location.X + contentLayer.Width - Width);
            RectangleF r = new RectangleF(-2, 0, rightShadowLayer.Width, rightShadowLayer.Height - 1);

            int alpha = (int)((rightRichWidth / contentLayer.Width) * 200f);
            LinearGradientBrush brush = new LinearGradientBrush(r, Color.FromArgb(0, 0, 0, 0), Color.FromArgb(alpha, 0, 0, 0), LinearGradientMode.Horizontal);
            graphis.FillRectangle(brush, r);
            brush.Dispose();
        }

        private void LeftShadowLayer_GDIPaint(GDIGraphics g)
        {
            if (!useHorShadow)
                return;

            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;

            float leftRichWidth = Math.Max(0, 0 - contentLayer.Location.X);
            RectangleF r = new RectangleF(0, 0, leftShadowLayer.Width - 2, leftShadowLayer.Height - 1);

            int alpha = (int)((leftRichWidth / contentLayer.Width) * 200f);
            LinearGradientBrush brush = new LinearGradientBrush(r, Color.FromArgb(alpha, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), LinearGradientMode.Horizontal);
            graphis.FillRectangle(brush, r);
            brush.Dispose();
        }

        public void SetHorShadowRects(RectangleF leftShadowRect, RectangleF rightShadowRect)
        {
            leftShadowLayer.Location = new PointF(leftShadowRect.X - 2, leftShadowRect.Y);
            leftShadowLayer.Width = leftShadowRect.Width;
            leftShadowLayer.Height = leftShadowRect.Height;

            rightShadowLayer.Location = new PointF(rightShadowRect.X + 2, rightShadowRect.Y);
            rightShadowLayer.Width = rightShadowRect.Width;
            rightShadowLayer.Height = rightShadowRect.Height;
        }

        public void SetVerShadowRects(RectangleF topShadowRect, RectangleF bottomShadowRect)
        {
            topShadowLayer.Location = new PointF(topShadowRect.X, topShadowRect.Y - 2);
            topShadowLayer.Width = topShadowRect.Width;
            topShadowLayer.Height = topShadowRect.Height;

            bottomShadowLayer.Location = new PointF(bottomShadowRect.X, bottomShadowRect.Y + 2);
            bottomShadowLayer.Width = bottomShadowRect.Width;
            bottomShadowLayer.Height = bottomShadowRect.Height;
        }

        public void MoveValueX(float v)
        {
            if (!canHorSlider)
                return;

            float x;
            float richWidth = Width / 2;
            float len;

            downWrapperLocationX = contentLayer.Location.X;
            len = downWrapperLocationX + v + richWidth - Width;

            if (len > 0)
            {
                x = GetContentPositionX(-len);
                contentLayer.Location = new PointF(x, contentLayer.Location.Y);

                Refresh();
            }
        }

        public void MoveValueY(float v)
        {
            if (!canVerSlider)
                return;

            float y;
            float richHeight = Height / 2;
            float len;

            downWrapperLocationY = contentLayer.Location.Y;
            len = downWrapperLocationY + v + richHeight - Height;

            if (len > 0)
            {
                y = GetContentPositionY(-len);
                contentLayer.Location = new PointF(contentLayer.Location.X, y);

                Refresh();
            }
        }

        private void Slider_MouseMove(object sender, ScMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                float offsetX;
                float offsetY;
                float x = contentLayer.Location.X;
                float y = contentLayer.Location.Y;

                if (canHorSlider)
                {
                    offsetX = e.Location.X - downWrapperX;
                    x = GetContentPositionX(offsetX);
                }

                if (canVerSlider)
                {
                    offsetY = e.Location.Y - downWrapperY;
                    y = GetContentPositionY(offsetY);
                }

                contentLayer.Location = new PointF(x, y);

                Refresh();
            }
        }

        float GetContentPositionX(float offsetX)
        {
            float x = downWrapperLocationX + offsetX;

            if (x > 0)
            {
                x = 0;
            }
            else if (x + contentLayer.Width < Width)
            {
                x = Width - contentLayer.Width;
            }

            return x;
        }


        float GetContentPositionY(float offsetY)
        {
            float y = downWrapperLocationY + offsetY;

            if (y > 0)
            {
                y = 0;
            }
            else if (y + contentLayer.Height < Height)
            {
                y = Height - contentLayer.Height;
            }

            return y;
        }


        private void Slider_MouseDown(object sender, ScMouseEventArgs e)
        {
            if (canHorSlider)
            {
                downWrapperX = e.Location.X;
                downWrapperLocationX = contentLayer.Location.X;
            }

            if (canVerSlider)
            {
                downWrapperY = e.Location.Y;
                downWrapperLocationY = contentLayer.Location.Y;
            }
        }
    }
}

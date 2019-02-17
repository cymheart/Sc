using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Sc
{
    public class ScVxPageTips:ScLayer
    {
        GraphicsPath tipsPath;
        RectangleF txtRect;

        public int alpha = 155;
        public Color bgColor = Color.WhiteSmoke;
        public Color txtColor = Color.Blue;
        public Color sideColor = Color.Gray;
        public string txt = "15";
        public Font txtFont = new Font("微软雅黑", 12);
        public float triWidthScale = 5;
        public float triHeightScale = 3;

        public bool isDisplaySide = false;
        public float sideWidth = 1.0f;

        public ScVxPageTips()
        {
            GDIPaint += ScVxPageTips_GDIPaint;
            SizeChanged += ScVxPageTips_SizeChanged;
        }

        private void ScVxPageTips_SizeChanged(object sender, SizeF oldSize)
        {
            if (tipsPath != null)
                tipsPath.Dispose();

            tipsPath = CreateTipsPath();                              
        }

        private void ScVxPageTips_GDIPaint(GDIGraphics g)
        {
            if (alpha == 0)
                return;

            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;
            graphis.TextRenderingHint = TextRenderingHint.AntiAlias;
 
            Color c = Color.FromArgb(alpha - alpha / 2, bgColor);
            Brush brush = new SolidBrush(c);

            graphis.FillPath(brush, tipsPath);
            brush.Dispose();

            if (isDisplaySide)
            {
                c = Color.FromArgb(alpha, sideColor);
                Pen pen = new Pen(c, sideWidth);
                graphis.DrawPath(pen, tipsPath);
                pen.Dispose();
            }


            //
            c = Color.FromArgb(alpha, txtColor);
            brush = new SolidBrush(c);
            DrawUtils.LimitBoxDraw(graphis, txt, txtFont, brush, txtRect, true, 0);
            brush.Dispose();

        }

        public GraphicsPath CreateTipsPath()
        {
            RectangleF rect = new RectangleF(1, 1, Width - 2, Height - 2);
            float cornerRadius = rect.Width / 12;
            float w = rect.Width / triWidthScale;
            float h = rect.Height / triHeightScale;
            float halfw = rect.Width / 2;

            txtRect = new RectangleF(0, 0, Width, Height - h);

            GraphicsPath tipsPath = new GraphicsPath();
            tipsPath.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            tipsPath.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            tipsPath.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            tipsPath.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2 - h);
            tipsPath.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2 - h, cornerRadius * 2, cornerRadius * 2, 0, 90);

            tipsPath.AddLine(rect.Right - cornerRadius * 2, rect.Bottom - h, rect.Right - cornerRadius * 2 - w, rect.Bottom - h);
            tipsPath.AddLine(rect.Right - cornerRadius * 2 - w, rect.Bottom - h, rect.Right - halfw, rect.Bottom );

            tipsPath.AddLine(rect.Right - halfw, rect.Bottom, rect.X + cornerRadius * 2 + w, rect.Bottom - h);
            tipsPath.AddLine(rect.X + cornerRadius * 2 + w, rect.Bottom - h, rect.X + cornerRadius * 2, rect.Bottom - h);
            tipsPath.AddArc(rect.X, rect.Bottom - cornerRadius * 2 - h, cornerRadius * 2, cornerRadius * 2, 90, 90);
            tipsPath.AddLine(rect.X, rect.Bottom - cornerRadius * 2 - h, rect.X, rect.Y + cornerRadius * 2);

            tipsPath.CloseFigure();
            return tipsPath;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Utils
{
    public class DrawTools
    {
        static public void LimitBoxDraw(
            Graphics g, string val, Font font, Brush brush, RectangleF rect,
            float offsetX, float offsetY, bool isCenterPosX, bool isCenterPosY, int direction)
        {
    
            PointF pt = new PointF(rect.X + offsetX, rect.Y + offsetY);
            float scalex = 1.0F;
            float scaley = 1.0F;
            float stringWidth;
            float stringHeight;
            StringFormat strF;
            SizeF fontSize;

            if (direction == 0)
            {
                strF = new StringFormat();
                fontSize = g.MeasureString(val, font);
            }
            else
            {
                strF = new StringFormat(StringFormatFlags.DirectionVertical);
                fontSize = g.MeasureString(val, font, 0, strF);
            }


            stringWidth = fontSize.Width;
            stringHeight = fontSize.Height;

            if (isCenterPosX)
            {
                pt.X = rect.X + rect.Width / 2 - stringWidth / 2 + offsetX;
            }

            if (isCenterPosY)
            {
                pt.Y = rect.Y + rect.Height / 2 - stringHeight / 2 + offsetY;
            }

            if (fontSize.Width > rect.Width)
            {
                scalex = rect.Width / fontSize.Width;
                stringWidth = rect.Width;

                if (isCenterPosX)
                    pt.X = rect.X + rect.Width / 2 - stringWidth / 2 + offsetX;
                else
                    pt.X = rect.X + offsetX;

                pt.X /= scalex;
            }

            if (fontSize.Height > rect.Height)
            {
                scaley = rect.Height / fontSize.Height;
                stringHeight = rect.Height;

                if (isCenterPosY)
                    pt.Y = rect.Y + rect.Height / 2 - stringHeight / 2 + offsetY;
                else
                    pt.Y = rect.Y + offsetY;

                pt.Y /= scaley;
            }


            Matrix matrix = new Matrix();

            if (brush == null)
                brush = new SolidBrush(Color.Black);

            if (scalex == 0) { scalex = 0.1F; }
            if (scaley == 0) { scaley = 0.1F; }

            matrix.Scale(scalex, scaley);
            g.Transform = matrix;

            g.DrawString(val, font, brush, pt, strF);
            matrix.Scale(1 / scalex, 1 / scaley);
            g.Transform = matrix;
        }

        static public void LimitBoxDraw(Graphics g, string val, Font font, Brush brush, RectangleF rect, float offsetX, float offsetY, bool isCenterPos, int direction)
        {
            LimitBoxDraw(g, val, font, brush, rect, offsetX, offsetY, isCenterPos, isCenterPos, direction);
        }

        static public void LimitBoxDraw(Graphics g, string val, Font font, RectangleF rect, float offsetX, float offsetY, bool isCenterPos, int direction)
        {
            LimitBoxDraw(g, val, font, null, rect, offsetX, offsetY, isCenterPos, isCenterPos, direction);
        }

        static public void LimitBoxDraw(Graphics g, string val, Font font, Brush brush, RectangleF rect, bool isCenterPos, int direction)
        {
            LimitBoxDraw(g, val, font, brush, rect, 0, 0, isCenterPos, isCenterPos, direction);
        }

        static public void LimitBoxDraw(Graphics g, string val, Font font, RectangleF rect, bool isCenterPos, int direction)
        {
            LimitBoxDraw(g, val, font, null, rect, 0, 0, isCenterPos, isCenterPos, direction);
        }

        static public void LimitBoxDraw(Graphics g, string val, Font font, RectangleF rect, bool isCenterPosX, bool isCenterPosY, int direction)
        {
            LimitBoxDraw(g, val, font, null, rect, 0, 0, isCenterPosX, isCenterPosY, direction);
        }

        public static void DrawRoundRectangle(Graphics g, Pen pen, RectangleF rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.DrawPath(pen, path);
            }
        }
        public static void FillRoundRectangle(Graphics g, Brush brush, RectangleF rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.FillPath(brush, path);
            }
        }
        internal static GraphicsPath CreateRoundedRectanglePath(RectangleF rect, int cornerRadius)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }
    }
}

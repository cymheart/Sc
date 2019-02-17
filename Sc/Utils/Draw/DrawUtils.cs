using Sc;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Utils
{
    public class DrawUtils
    {
        static public void LimitBoxDraw(
         Graphics g, string val, Font font, System.Drawing.Brush brush, System.Drawing.RectangleF rect,
         DrawPositionType drawPosType, bool isLimitDraw, bool isMultipleRow, int direction)
        {
            PointF pt = new PointF(rect.X, rect.Y);
            float scalex = 1.0F;
            float scaley = 1.0F;
            float stringWidth;
            float stringHeight;
            StringFormat strF;
            SizeF fontSize;
            System.Drawing.RectangleF layoutRect = rect;

            if (direction == 0)
            {
                strF = new StringFormat();

                if (!isMultipleRow)
                    fontSize = g.MeasureString(val, font);
                else
                {
                    fontSize = g.MeasureString(val, font, (int)rect.Width);
                    layoutRect.Width = rect.Width;
                    layoutRect.Height = fontSize.Height;
                }

            }
            else
            {
                strF = new StringFormat(StringFormatFlags.DirectionVertical);

                if (!isMultipleRow)
                    fontSize = g.MeasureString(val, font, 0, strF);
                else
                {
                    fontSize = g.MeasureString(val, font, (int)rect.Height);
                    layoutRect.Width = rect.Width;
                    layoutRect.Height = fontSize.Height;
                }
            }

            stringWidth = fontSize.Width;
            stringHeight = fontSize.Height;

            if (isLimitDraw)
            {
                if (fontSize.Width > rect.Width)
                {
                    scalex = rect.Width / fontSize.Width;
                    stringWidth = rect.Width;
                }

                if (fontSize.Height > rect.Height)
                {
                    scaley = rect.Height / fontSize.Height;
                    stringHeight = rect.Height;
                }
            }



            switch (drawPosType)
            {
                case DrawPositionType.LeftTop:
                    pt.X = rect.X;
                    pt.Y = rect.Y;
                    break;

                case DrawPositionType.Top:
                    pt.X = rect.X + rect.Width / 2 - stringWidth / 2;
                    pt.Y = rect.Y;
                    break;

                case DrawPositionType.RightTop:
                    pt.X = rect.Right - stringWidth;
                    pt.Y = rect.Y;
                    break;

                case DrawPositionType.Left:
                    pt.X = rect.X;
                    pt.Y = rect.Y + rect.Height / 2 - stringHeight / 2;
                    break;

                case DrawPositionType.Center:
                    pt.X = rect.X + rect.Width / 2 - stringWidth / 2;
                    pt.Y = rect.Y + rect.Height / 2 - stringHeight / 2;
                    break;

                case DrawPositionType.Right:
                    pt.X = rect.Right - stringWidth;
                    pt.Y = rect.Y + rect.Height / 2 - stringHeight / 2;
                    break;

                case DrawPositionType.LeftBottom:
                    pt.X = rect.X;
                    pt.Y = rect.Bottom - stringHeight;
                    break;

                case DrawPositionType.Bottom:
                    pt.X = rect.X + rect.Width / 2 - stringWidth / 2;
                    pt.Y = rect.Bottom - stringHeight;
                    break;

                case DrawPositionType.RightBottom:
                    pt.X = rect.Right - stringWidth;
                    pt.Y = rect.Bottom - stringHeight;
                    break;
            }

            pt.X /= scalex;
            pt.Y /= scaley;


            System.Drawing.Drawing2D.Matrix matrix, oldMatrix;

            if (brush == null)
                brush = new SolidBrush(System.Drawing.Color.Black);

            if (scalex == 0) { scalex = 0.1F; }
            if (scaley == 0) { scaley = 0.1F; }

            oldMatrix = g.Transform;

            matrix = oldMatrix.Clone();
            matrix.Scale(scalex, scaley);

            g.Transform = matrix;

            if (!isMultipleRow)
                g.DrawString(val, font, brush, pt, strF);
            else
            {
                layoutRect.X = pt.X;
                layoutRect.Y = pt.Y;
                g.DrawString(val, font, brush, layoutRect, strF);
            }

            g.Transform = oldMatrix;
        }

        static public void LimitBoxDraw(
          Graphics g, string val, Font font, System.Drawing.Brush brush, System.Drawing.RectangleF rect,
          bool isCenterPosX, bool isCenterPosY, int direction)
        {
            DrawPositionType drawPosType = DrawPositionType.LeftTop;
            if (isCenterPosX && isCenterPosY)
                drawPosType = DrawPositionType.Center;
            else if (isCenterPosX && !isCenterPosY)
                drawPosType = DrawPositionType.Top;
            else if (!isCenterPosX && isCenterPosY)
                drawPosType = DrawPositionType.Left;

            LimitBoxDraw(g, val, font, brush, rect, drawPosType, true, false, direction);
        }

        static public void LimitBoxDraw(Graphics g, string val, Font font, System.Drawing.Brush brush, System.Drawing.RectangleF rect, bool isCenterPos, int direction)
        {
            DrawPositionType drawPosType = DrawPositionType.LeftTop;
            if (isCenterPos)
                drawPosType = DrawPositionType.Center;

            LimitBoxDraw(g, val, font, brush, rect, drawPosType, true, false, direction);
        }

        static public void LimitBoxDraw(Graphics g, string val, Font font, System.Drawing.RectangleF rect, bool isCenterPos, int direction)
        {
            DrawPositionType drawPosType = DrawPositionType.LeftTop;
            if (isCenterPos)
                drawPosType = DrawPositionType.Center;

            LimitBoxDraw(g, val, font, null, rect, drawPosType, true, false, direction);
        }

        static public void LimitBoxDraw(Graphics g, string val, Font font, System.Drawing.RectangleF rect, bool isCenterPosX, bool isCenterPosY, int direction)
        {
            DrawPositionType drawPosType = DrawPositionType.LeftTop;
            if (isCenterPosX && isCenterPosY)
                drawPosType = DrawPositionType.Center;
            else if(isCenterPosX && !isCenterPosY)
                drawPosType = DrawPositionType.Top;
            else if (!isCenterPosX && isCenterPosY)
                drawPosType = DrawPositionType.Left;

            LimitBoxDraw(g, val, font, null, rect, drawPosType, true, false, direction);
        }

        public static void DrawRoundRectangle(Graphics g, Pen pen, System.Drawing.RectangleF rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.DrawPath(pen, path);
            }
        }
        public static void FillRoundRectangle(Graphics g, System.Drawing.Brush brush, System.Drawing.RectangleF rect, float cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.FillPath(brush, path);
            }
        }
        public static GraphicsPath CreateRoundedRectanglePath(System.Drawing.RectangleF rect, float cornerRadius)
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

        public static GraphicsPath CreateOutlineRoundedRectanglePath(System.Drawing.RectangleF rect, float cornerRadius, float w)
        {
            GraphicsPath roundedRect = new GraphicsPath();

            roundedRect.StartFigure();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();


            rect = new System.Drawing.RectangleF(rect.X + w, rect.Y + w, rect.Width - 2 * w, rect.Height - 2 * w);
            roundedRect.StartFigure();
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

        public static PathGeometry CreateRoundRectGeometry(D2DGraphics g, RawRectangleF rect, float r)
        {
            PathGeometry pathGeometry = new PathGeometry(D2DGraphics.d2dFactory);

            GeometrySink pSink = null;
            pSink = pathGeometry.Open();
            pSink.SetFillMode(SharpDX.Direct2D1.FillMode.Alternate);

            pSink.BeginFigure(new RawVector2(rect.Left, rect.Top + r), FigureBegin.Filled);
            AddRect(pSink, rect, r);
            pSink.EndFigure(FigureEnd.Closed);
 
            pSink.Close();
            pSink.Dispose();

            return pathGeometry;
        }


        public static PathGeometry CreateOutlineRoundRectGeometry(D2DGraphics g, RawRectangleF rect, float r)
        {
            PathGeometry pathGeometry = new PathGeometry(D2DGraphics.d2dFactory);

            GeometrySink pSink = null;
            pSink = pathGeometry.Open();
            pSink.SetFillMode(SharpDX.Direct2D1.FillMode.Alternate);

            pSink.BeginFigure(new RawVector2(rect.Left, rect.Top + r), FigureBegin.Filled);
            AddRect(pSink, rect, r);
            pSink.EndFigure(FigureEnd.Closed);

            RawRectangleF rect2 = new RawRectangleF(rect.Left + 0.5f, rect.Top + 0.5f, rect.Right - 0.5f, rect.Bottom - 0.5f);
            pSink.BeginFigure(new RawVector2(rect2.Left, rect2.Top + r), FigureBegin.Filled);
            AddRect(pSink, rect2, r);
            pSink.EndFigure(FigureEnd.Closed);

            pSink.Close();
            pSink.Dispose();

            return pathGeometry;
        }


        static void AddRect(GeometrySink pSink, RawRectangleF rect, float r)
        {
            ArcSegment arcSeg = new ArcSegment();
            arcSeg.ArcSize = ArcSize.Small;
            arcSeg.Point = new RawVector2(rect.Left + r, rect.Top);
            arcSeg.RotationAngle = 90;
            arcSeg.Size = new Size2F(r, r);
            arcSeg.SweepDirection = SweepDirection.Clockwise;
            pSink.AddArc(arcSeg);

            pSink.AddLine(new RawVector2(rect.Right - r, rect.Top));

            //
            arcSeg.Point = new RawVector2(rect.Right, rect.Top + r);
            pSink.AddArc(arcSeg);
            pSink.AddLine(new RawVector2(rect.Right, rect.Bottom - r));


            //
            arcSeg.Point = new RawVector2(rect.Right - r, rect.Bottom);
            pSink.AddArc(arcSeg);
            pSink.AddLine(new RawVector2(rect.Left + r, rect.Bottom));

            arcSeg.Point = new RawVector2(rect.Left, rect.Bottom - r);
            pSink.AddArc(arcSeg);
            pSink.AddLine(new RawVector2(rect.Left, rect.Top + r));
        }
    }
}

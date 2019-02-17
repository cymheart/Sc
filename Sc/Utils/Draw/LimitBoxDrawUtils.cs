using System.Drawing;
using System.Windows.Forms;

namespace Utils
{
    public enum DrawPositionType
    {
        LeftTop,
        Top,
        RightTop,
        Left,
        Center,
        Right,
        LeftBottom,
        Bottom,
        RightBottom
    }


    public class LimitBoxDrawInfo
    {
        public Graphics g;
        public string txt;
        public Font font;
        public Brush brush = Brushes.Black;
        public RectangleF rect;
        public DrawPositionType drawPosType = DrawPositionType.Center;
        public bool isLimitDraw = true;
        public bool isMultipleRow = false;
        public int direction = 0;
        public float rotateAngle = 0;
    }

    public class LimitBoxDrawUtils
    {
        static public void LimitBoxDraw(LimitBoxDrawInfo drawInfo)
        {
            PointF pt = new PointF(drawInfo.rect.X, drawInfo.rect.Y);
            float scalex = 1.0F;
            float scaley = 1.0F;
            float stringWidth;
            float stringHeight;
            StringFormat strF;
            SizeF fontSize;
            System.Drawing.RectangleF layoutRect = drawInfo.rect;
            Graphics g = drawInfo.g;

            if (drawInfo.direction == 0)
            {
                strF = new StringFormat();

                if (!drawInfo.isMultipleRow)
                {
                    fontSize = g.MeasureString(drawInfo.txt, drawInfo.font);
                }
                else
                {
                    SizeF area = new SizeF(drawInfo.rect.Width, 0);
                    fontSize = g.MeasureString(drawInfo.txt, drawInfo.font, area);
                    layoutRect.Width = fontSize.Width;
                    layoutRect.Height = fontSize.Height;
                }
            }
            else
            {
                strF = new StringFormat(StringFormatFlags.DirectionVertical);

                if (!drawInfo.isMultipleRow)
                {
                    fontSize = g.MeasureString(drawInfo.txt, drawInfo.font, 0, strF);
                }
                else
                {
                    SizeF area = new SizeF(0, drawInfo.rect.Height);
                    fontSize = g.MeasureString(drawInfo.txt, drawInfo.font, area, strF);
                    layoutRect.Width = fontSize.Width;
                    layoutRect.Height = fontSize.Height;
                }
            }

            stringWidth = fontSize.Width;
            stringHeight = fontSize.Height;

            if (drawInfo.isLimitDraw)
            {
                if (fontSize.Width > drawInfo.rect.Width)
                {
                    scalex = drawInfo.rect.Width / fontSize.Width;
                    stringWidth = drawInfo.rect.Width;
                }

                if (fontSize.Height > drawInfo.rect.Height)
                {
                    scaley = drawInfo.rect.Height / fontSize.Height;
                    stringHeight = drawInfo.rect.Height;
                }
            }


            switch (drawInfo.drawPosType)
            {
                case DrawPositionType.LeftTop:
                    pt.X = drawInfo.rect.X;
                    pt.Y = drawInfo.rect.Y;
                    break;

                case DrawPositionType.Top:
                    pt.X = drawInfo.rect.X + drawInfo.rect.Width / 2 - stringWidth / 2;
                    pt.Y = drawInfo.rect.Y;
                    break;

                case DrawPositionType.RightTop:
                    pt.X = drawInfo.rect.Right - stringWidth;
                    pt.Y = drawInfo.rect.Y;
                    break;

                case DrawPositionType.Left:
                    pt.X = drawInfo.rect.X;
                    pt.Y = drawInfo.rect.Y + drawInfo.rect.Height / 2 - stringHeight / 2;
                    break;

                case DrawPositionType.Center:
                    pt.X = drawInfo.rect.X + drawInfo.rect.Width / 2 - stringWidth / 2;
                    pt.Y = drawInfo.rect.Y + drawInfo.rect.Height / 2 - stringHeight / 2;
                    break;

                case DrawPositionType.Right:
                    pt.X = drawInfo.rect.Right - stringWidth;
                    pt.Y = drawInfo.rect.Y + drawInfo.rect.Height / 2 - stringHeight / 2;
                    break;

                case DrawPositionType.LeftBottom:
                    pt.X = drawInfo.rect.X;
                    pt.Y = drawInfo.rect.Bottom - stringHeight;
                    break;

                case DrawPositionType.Bottom:
                    pt.X = drawInfo.rect.X + drawInfo.rect.Width / 2 - stringWidth / 2;
                    pt.Y = drawInfo.rect.Bottom - stringHeight;
                    break;

                case DrawPositionType.RightBottom:
                    pt.X = drawInfo.rect.Right - stringWidth;
                    pt.Y = drawInfo.rect.Bottom - stringHeight;
                    break;
            }

            pt.X /= scalex;
            pt.Y /= scaley;


            System.Drawing.Drawing2D.Matrix matrix, oldMatrix;

            if (drawInfo.brush == null)
                drawInfo.brush = new SolidBrush(System.Drawing.Color.Black);

            if (scalex == 0) { scalex = 0.1F; }
            if (scaley == 0) { scaley = 0.1F; }

            oldMatrix = g.Transform;

            matrix = oldMatrix.Clone();
            matrix.Scale(scalex, scaley);

            float x = pt.X + stringWidth / 2;
            float y = pt.Y + stringHeight / 2;

            matrix.Translate(x, y);
            matrix.Rotate(drawInfo.rotateAngle);

            g.Transform = matrix;

            if (!drawInfo.isMultipleRow)
            {
                g.DrawString(drawInfo.txt, drawInfo.font, drawInfo.brush, -stringWidth / 2, -stringHeight / 2, strF);
            }
            else
            {
                layoutRect.X = -stringWidth / 2;
                layoutRect.Y = -stringHeight / 2;
                g.DrawString(drawInfo.txt, drawInfo.font, drawInfo.brush, layoutRect, strF);
            }

            g.Transform = oldMatrix;
        }


        static public void LimitBoxDraw(
         Graphics g, string val, Font font, Brush brush, RectangleF rect,
         DrawPositionType drawPosType = DrawPositionType.Center, bool isLimitDraw = true,
         bool isMultipleRow = false, int direction = 0, float rotateAngle = 0)
        {
            LimitBoxDrawInfo drawInfo = new LimitBoxDrawInfo();
            drawInfo.g = g;
            drawInfo.txt = val;
            drawInfo.font = font;
            drawInfo.brush = brush;
            drawInfo.rect = rect;
            drawInfo.drawPosType = drawPosType;
            drawInfo.isLimitDraw = isLimitDraw;
            drawInfo.isMultipleRow = isMultipleRow;
            drawInfo.direction = direction;
            drawInfo.rotateAngle = rotateAngle;

            LimitBoxDraw(drawInfo);
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
    }
}

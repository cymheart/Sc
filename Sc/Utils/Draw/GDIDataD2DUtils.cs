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

namespace Sc
{
    public class GDIDataD2DUtils
    {
        static public System.Drawing.RectangleF UnionRectF(System.Drawing.RectangleF orgRect, System.Drawing.RectangleF unionRect)
        {
            float left, top, right, bottom;

            if (orgRect.Left <= unionRect.Left)
                left = orgRect.Left;
            else
                left = unionRect.Left;


            if (orgRect.Top <= unionRect.Top)
                top = orgRect.Top;
            else
                top = unionRect.Top;


            if (orgRect.Right >= unionRect.Right)
               right = orgRect.Right;
            else
               right = unionRect.Right;


            if (orgRect.Bottom >= unionRect.Bottom)
                bottom = orgRect.Bottom;
            else
                bottom = unionRect.Bottom;

            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(left, top, right - left, bottom - top);

            return rect;
        }


        static public RawRectangleF TransToRawRectF(System.Drawing.RectangleF rect)
        {
            RawRectangleF rawRectF =
                new RawRectangleF(
                rect.Left, rect.Top,
                rect.Right, rect.Bottom);

            return rawRectF;
        }

        static public System.Drawing.Drawing2D.Matrix TransRawMatrix3x2ToMatrix(RawMatrix3x2 m)
        {
            System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix(m.M11, m.M12, m.M21, m.M22, m.M31, m.M32);
            return matrix;
        }

        static public RawMatrix3x2 TransMatrixToRawMatrix3x2(System.Drawing.Drawing2D.Matrix m)
        {
            float[] val = m.Elements;
            RawMatrix3x2 rawMatrix = new RawMatrix3x2(val[0], val[1], val[2], val[3], val[4], val[5]);
            return rawMatrix;   
        }

        static public RawColor4 TransToRawColor4(System.Drawing.Color color)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;
            float a = color.A / 255f;

            RawColor4 rawColor4 = new RawColor4(r, g, b, a);
            return rawColor4;
        }

        static public RawVector2 TransToRawVector2(PointF pt)
        {
            return new RawVector2(pt.X, pt.Y);
        }


        static public System.Drawing.RectangleF GetBoundBox(PointF[] pts)
        {
            PointF left, top, right, bottom;
            left = top = right = bottom = pts[0];

            for (int i = 1; i < pts.Count(); i++)
            {
                if (pts[i].X < left.X)
                    left = pts[i];

                if (pts[i].X > right.X)
                    right = pts[i];

                if (pts[i].Y < top.Y)
                    top = pts[i];

                if (pts[i].Y > bottom.Y)
                    bottom = pts[i];
            }

            System.Drawing.RectangleF boundBox = new System.Drawing.RectangleF(left.X, top.Y, right.X - left.X, bottom.Y - top.Y);
            return boundBox;
        }

    }
}


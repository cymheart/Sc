using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    public class ScWrapper:ScLayer
    {
        public ScWrapper()
        {
           
        }

        public ScWrapper(int w, int h)
        {
            Width = w;
            Height = h;
        }

        public void FixSize()
        {
            RectangleF totalRect = Rectangle.Empty;

            foreach (ScLayer control in controls)
            {
                if (totalRect == RectangleF.Empty)
                {
                    totalRect = new RectangleF(
                        control.Location.X, control.Location.Y,
                        control.Width, control.Height);
                    continue;
                }

                totalRect = GDIDataD2DUtils.UnionRectF(totalRect, control.DirectionRect);
            }

            Width = totalRect.Right;
            Height = totalRect.Bottom;
        }

    }
}

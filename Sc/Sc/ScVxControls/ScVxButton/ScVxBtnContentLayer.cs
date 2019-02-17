using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Sc
{
    public class ScVxBtnContentLayer : ScLayer
    {
        protected override GraphicsPath CreateHitGeometryByGDIPLUS(GDIGraphics g)
        {
            RectangleF rect = new RectangleF(0, 0, Width - 1, Height - 1);
            GraphicsPath path = DrawUtils.CreateRoundedRectanglePath(rect, 6);
            return path;
        }
    }
}

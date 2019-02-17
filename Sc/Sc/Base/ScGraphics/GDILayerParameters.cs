using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    public class GDILayerParameters
    {
        public RectangleF ContentBounds;
        public float Opacity;
        public GraphicsPath GeometricMask;
        public GDIAntialiasMode MaskAntialiasMode;
        public RectangleF ClipRect;
        public RectangleF parentClipRect;

        public ScLayer sclayer;
    }
}

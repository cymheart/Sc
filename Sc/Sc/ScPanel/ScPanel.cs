using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    public class ScPanel: ScLayer
    {



        public ScPanel(ScMgr scmgr = null)
            :base(scmgr)
        {
            ScShadow shadow = new ScShadow(scmgr);
            shadow.CornersRadius = 4;
            shadow.ShadowRadius = 5;
            shadow.ShadowColor = Color.FromArgb(25, 0, 0, 0);
            ShadowLayer = shadow;

            LocationChanged += ScPanel_LocationChanged;
            SizeChanged += ScPanel_SizeChanged;
            D2DPaint += ScPanel_D2DPaint;
        }

        private void ScPanel_LocationChanged(object sender, PointF oldLocation)
        {
            if (Width == 0 || Height == 0)
                return;

            if (ShadowLayer != null)
            {
                ScShadow shadow = (ScShadow)ShadowLayer;
                shadow.Location = new PointF(DirectionRect.X - shadow.ShadowRadius, DirectionRect.Y - shadow.ShadowRadius );
            }
        }

        private void ScPanel_SizeChanged(object sender, SizeF oldSize)
        {
            if (Width == 0 || Height == 0)
                return;

            if (ShadowLayer != null)
            {
                ScShadow shadow = (ScShadow)ShadowLayer;
                shadow.DirectionRect = new RectangleF(DirectionRect.X - shadow.ShadowRadius, DirectionRect.Y - shadow.ShadowRadius, DirectionRect.Width + shadow.ShadowRadius * 2, DirectionRect.Height + shadow.ShadowRadius * 2);
            }
        }


        private void ScPanel_D2DPaint(D2DGraphics g)
        {
            g.RenderTarget.AntialiasMode = AntialiasMode.Aliased;

            RawRectangleF rect = new RawRectangleF(0, 1, Width - 1, Height - 1);
            RawColor4 rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(255, 241, 251, 253));
            SolidColorBrush brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.FillRectangle(rect, brush);

            rect = new RawRectangleF(0, 0, Width - 1, 10 - 1);
            rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(255, 122, 151, 207));
            brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.FillRectangle(rect, brush);


            rect = new RawRectangleF(1, 1, Width - 1, Height - 1);
            rawColor = GDIDataD2DUtils.TransToRawColor4(Color.FromArgb(255, 214, 215, 220));
            brush = new SolidColorBrush(g.RenderTarget, rawColor);
            g.RenderTarget.DrawRectangle(rect, brush);
        }
    }
}

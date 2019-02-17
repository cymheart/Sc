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
    public class GDIGraphics:ScGraphics
    {
        Graphics gdiGraph;
        Graphics rootGdiGraph;
        Control control;
        int w, h;

        LinkedList<GDILayer> layerStack = new LinkedList<GDILayer>();
        LinkedList<GDILayerParameters> paramStack = new LinkedList<GDILayerParameters>();

        public GDIGraphics(Graphics gdiGraph)
        {
            rootGdiGraph = gdiGraph;
            this.gdiGraph = gdiGraph;
        }

        public GDIGraphics(Control control)
        {
            this.control = control;
            w = control.Width;
            h = control.Height;
            rootGdiGraph = Graphics.FromHwnd(control.Handle);
            gdiGraph = rootGdiGraph;
        }

        public GDIGraphics(Bitmap bitmap)
        {
            rootGdiGraph = Graphics.FromImage(bitmap);
            gdiGraph = rootGdiGraph;

            w = bitmap.Width;
            h = bitmap.Height;
        }


        public Graphics GdiGraph
        {
            get { return gdiGraph; }   
        }

        public Graphics RootGdiGraph
        {
            set
            {
                rootGdiGraph = value;
                gdiGraph = rootGdiGraph;
            }
        }

        public override GraphicsType GetGraphicsType()
        {
            return GraphicsType.GDIPLUS;
        }

        public override void ResetClip()
        {
            gdiGraph.ResetClip();
        }

        public override void ResetTransform()
        {
            gdiGraph.ResetTransform();
        }

        public override void SetClip(RectangleF clipRect)
        {
            gdiGraph.SetClip(clipRect);
        }

        public override void TranslateTransform(float dx, float dy)
        {
            gdiGraph.TranslateTransform(dx, dy);
        }

        public override Matrix Transform
        {
            get
            {
                return gdiGraph.Transform;
            }

            set
            {
                gdiGraph.Transform = value;
            }
        }

        public void PushLayer(GDILayerParameters layerParameters, GDILayer layer)
        {
            layerStack.AddFirst(layer);
            paramStack.AddFirst(layerParameters);

            float w = (float)Math.Round(layerParameters.ContentBounds.Width);
            float h = (float)Math.Round(layerParameters.ContentBounds.Height);

            Bitmap bitmap = new Bitmap((int)w, (int)h);
            gdiGraph = layer.CreateGraphics(bitmap);
        }


        public void PopLayer()
        {
            GDILayer layer = layerStack.First();
            GDILayerParameters layerParameters = paramStack.First();
            RectangleF parentContentBound;
            RectangleF clipRect;
            ScLayer sclayer = layerParameters.sclayer;

            layerStack.RemoveFirst();
            paramStack.RemoveFirst();

            if (layerStack.Count > 0)
            {
                gdiGraph = layerStack.First().graph;
                parentContentBound = paramStack.First().ContentBounds;
                clipRect = paramStack.First().ClipRect;
            }
            else
            {
                gdiGraph = rootGdiGraph;
                parentContentBound = new RectangleF(0, 0, w, h);
                clipRect = layerParameters.parentClipRect;
            }


            float x = (float)Math.Round(layerParameters.ContentBounds.X - parentContentBound.X);
            float y = (float)Math.Round(layerParameters.ContentBounds.Y - parentContentBound.Y);

            Bitmap effectbmp = layer.bitmap;

          
            //后处理效果
            if(sclayer.UsePosttreatmentEffect)
            {
                effectbmp = sclayer.ScPostTreatmentEffectGDI(layer.bitmap);
            }
 
            //
            TextureBrush br = new TextureBrush(effectbmp, WrapMode.Clamp);  
            br.TranslateTransform((int)x, (int)y);

            switch(layerParameters.MaskAntialiasMode)
            {
                case GDIAntialiasMode.PerPrimitive:
                    gdiGraph.SmoothingMode = SmoothingMode.AntiAlias;
                    break;

                case GDIAntialiasMode.Aliased:
                    gdiGraph.SmoothingMode = SmoothingMode.None;
                    break;
            }
    
            gdiGraph.Transform = new Matrix();

            gdiGraph.SetClip(clipRect);
            gdiGraph.FillPath(br, layerParameters.GeometricMask);
            gdiGraph.ResetClip();

            //后处理效果释放
            if (sclayer.UsePosttreatmentEffect)
            {
               sclayer.ScReleasePostTreatmentEffectGDI(effectbmp);
            }

            br.Dispose(); 
        }
    }
}

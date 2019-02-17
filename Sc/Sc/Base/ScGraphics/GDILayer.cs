using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sc
{
    public class GDILayer : IDisposable
    {
        GDIGraphics gdiGraph;
        public Bitmap bitmap;
        public Graphics graph { get; set; }

        public GDILayer(GDIGraphics gdiGraph)
        {
            this.gdiGraph = gdiGraph;
        }


        public Graphics CreateGraphics(Bitmap bitmap)
        {
            this.bitmap = bitmap;
            graph = Graphics.FromImage(bitmap);
            return graph;
        }

        public void Dispose()
        {
            bitmap.Dispose();
            graph.Dispose();
        }
    }
}

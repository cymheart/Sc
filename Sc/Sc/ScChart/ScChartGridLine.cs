using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    public class ScChartGridLine:ScLayer
    {
        PointF startPoint;
        PointF endPoint;
        Pen linePen;

        ScChartGridLine()
        {
            linePen = new Pen(Color.Gray, 1f);

            this.GDIPaint += ScChartGridLine_GDIPaint;
        }

        private void ScChartGridLine_GDIPaint(GDIGraphics g)
        {
            Graphics gdiGraph = g.GdiGraph;
            gdiGraph.SmoothingMode = SmoothingMode.HighQuality;// 指定高质量、低速度呈现。
            gdiGraph.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            gdiGraph.DrawLine(linePen, startPoint, endPoint);
        }
    }
}

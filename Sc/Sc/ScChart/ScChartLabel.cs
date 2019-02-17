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
    public class ScChartLabel:ScLayer
    {
        Font textFont = new Font("微软雅黑", 15);
        string text = "";
        ScChartLabel()
        {
            GDIPaint += ScChartLabel_GDIPaint;
        }

        private void ScChartLabel_GDIPaint(GDIGraphics g)
        {
            Graphics gdiGraph = g.GdiGraph;
            gdiGraph.SmoothingMode = SmoothingMode.HighQuality;// 指定高质量、低速度呈现。
            gdiGraph.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            RectangleF rect = new RectangleF(0, 0, Width, Height);
            Utils.LimitBoxDrawUtils.LimitBoxDraw(gdiGraph, text, textFont, rect, true, 0); 
        }
    }
}

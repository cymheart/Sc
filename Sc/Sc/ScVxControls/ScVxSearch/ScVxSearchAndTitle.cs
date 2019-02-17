using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Sc
{
    public class ScVxSearchAndTitle:ScLayer
    {
        Table mainTable;
        ScVxSearch search;

        public string title;
        public Color titleColor = Color.FromArgb(255, 125, 175, 175);
        Font titlefont = new Font("微软雅黑", 12);
        public ScVxSearchAndTitle()
        {
            search = new ScVxSearch();
            Add(search);
   
            SizeChanged += ScVxSearchAndTitle_SizeChanged;

            GDIPaint += ScVxSearchAndTitle_GDIPaint;
        }

        public void SetSearchEvent(ScVxSearch.SearchEventHandler searchEvent)
        {
            search.SetSerachEvent(searchEvent);
        }

        private void ScVxSearchAndTitle_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;
            graphis.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            RectangleF rect = mainTable.GetCellContentRect(0, 0);

            Brush brush = new SolidBrush(titleColor);
            DrawUtils.LimitBoxDraw(graphis, title, titlefont, brush, rect, false, true, 0);
            brush.Dispose();
        }

        private void ScVxSearchAndTitle_SizeChanged(object sender, SizeF oldSize)
        {
            RectangleF rect = new RectangleF(130, 18, Width - 150 , Height - 40);
            mainTable = new Table(rect, 2, 1);

            TableLine tableLine = new TableLine(LineDir.HORIZONTAL);
            tableLine.lineComputeMode = LineComputeMode.PERCENTAGE;

            tableLine.computeParam = 0.4f;
            mainTable.SetLineArea(0, tableLine);

            tableLine.computeParam = 0.6f;
            mainTable.SetLineArea(1, tableLine);

            mainTable.ComputeLinesArea(LineDir.HORIZONTAL);

            rect = mainTable.GetCellContentRect(1, 0);
            search.Width = rect.Width;
            search.Height = rect.Height;
            search.Location = new PointF(rect.X, rect.Y);

        }
    }
}

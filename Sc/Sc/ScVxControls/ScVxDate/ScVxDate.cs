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
    public class ScVxDate :ScLayer
    {
        Table mainTable;
        Font font = new Font("微软雅黑", 10);
        public Color bgColor = Color.FromArgb(220, 29, 39, 49);
        public Color txtColor = Color.FromArgb(255, 125, 175, 175);

        ScAnimation dateAnim;
        DateTime dt;
        public ScVxDate()
        {
            GDIPaint += ScVxDate_GDIPaint;
            SizeChanged += ScVxDate_SizeChanged;

            dateAnim = new ScAnimation(this, true);
            dateAnim.DurationMS = 1000;

            dateAnim.AnimationEvent += DateAnim_AnimationEvent;
        }

        ~ScVxDate()
        {
            dateAnim.Stop();
        }

        private void DateAnim_AnimationEvent(ScAnimation scAnimation)
        {
            dt = DateTime.Now;
            Refresh();
        }

        private void ScVxDate_SizeChanged(object sender, SizeF oldSize)
        {
            RectangleF rect = new RectangleF(0, 0, Width, Height);
            mainTable = new Table(rect, 1, 3);

            TableLine tableLine = new TableLine(LineDir.VERTICAL);
            tableLine.lineComputeMode = LineComputeMode.PERCENTAGE;

            tableLine.computeParam = 0.4f;
            mainTable.SetLineArea(0, tableLine);

            tableLine.computeParam = 0.25f;
            mainTable.SetLineArea(1, tableLine);

            tableLine.computeParam = 0.35f;
            mainTable.SetLineArea(2, tableLine);
            mainTable.ComputeLinesArea(LineDir.VERTICAL);

            dateAnim.Stop();
            dateAnim.Start();

        }

        private void ScVxDate_GDIPaint(GDIGraphics g)
        {
            Graphics graphis = g.GdiGraph;
            graphis.SmoothingMode = SmoothingMode.HighQuality;
            graphis.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            RectangleF rect = new RectangleF(0, 0, Width -1, Height -1);

            //
            Brush brush = new SolidBrush(bgColor);
            DrawUtils.FillRoundRectangle(graphis, brush, rect, 6);

            Pen pen = new Pen(txtColor);
            DrawUtils.DrawRoundRectangle(graphis, pen, rect, 6);

            //
            brush = new SolidBrush(Color.FromArgb(105, 163, 175));

            rect = mainTable.GetCellContentRect(0, 0);
            string d = dt.ToString("yyyy-MM-dd");
            DrawUtils.LimitBoxDraw(graphis, d, font, brush, rect, true, 0);

            //
            rect = mainTable.GetCellContentRect(0, 1);
            d = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek);
            DrawUtils.LimitBoxDraw(graphis, d, font, brush, rect, true, 0);


            //
            rect = mainTable.GetCellContentRect(0, 2);
            d = DateTime.Now.ToLongTimeString().ToString();
            DrawUtils.LimitBoxDraw(graphis, d, font, Brushes.WhiteSmoke, rect, true, 0);

            brush.Dispose();

            //
            LinePos linePos = mainTable.GetColLinePos(0);
            linePos.start = new PointF((int)linePos.start.X, (int)(linePos.start.Y + Height / 7));
            linePos.end = new PointF((int)linePos.end.X, (int)(linePos.end.Y - Height / 7));
            graphis.DrawLine(pen, linePos.start, linePos.end);

            //
            linePos = mainTable.GetColLinePos(1);
            linePos.start = new PointF((int)linePos.start.X, (int)(linePos.start.Y + Height / 7));
            linePos.end = new PointF((int)linePos.end.X, (int)(linePos.end.Y - Height / 7));
            graphis.DrawLine(pen, linePos.start, linePos.end);


        }
    }
}
